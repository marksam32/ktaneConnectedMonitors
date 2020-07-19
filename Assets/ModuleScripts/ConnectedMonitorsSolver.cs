using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ConnectedMonitors
{
    public class ConnectedMonitorsSolver
    {
	    private readonly IConnectedMonitorsRandom _rnd;
	    private readonly List<Monitor> _monitors;
        private readonly IConnectedMonitorsLogger _logger;
        private IList<Monitor> _monitorPressOrder;
        private MonitorPressContext _monitorPressContext;
        private readonly int _startIndex;
        private readonly DebugPressContext _debugPressContext;

        public ConnectedMonitorsSolver(IConnectedMonitorsLogger logger, List<Monitor> monitors, IConnectedMonitorsRandom rnd) : this(logger, monitors, 11, new DebugPressContext(), rnd){}

        internal ConnectedMonitorsSolver(IConnectedMonitorsLogger logger, List<Monitor> monitors, int startIndex, DebugPressContext debugPressContext, IConnectedMonitorsRandom rnd)
        {
            _logger = logger;
            _monitors = monitors;
            _startIndex = startIndex;
            _debugPressContext = debugPressContext;
            _rnd = rnd;
        }

        public ReadOnlyCollection<IndicatorInfo> GetAllIndicators()
        {
            var indicators = _monitors.SelectMany(x => x.Indicators).GroupBy(x => x.GlobalIndex).Select(x => x.First()).OrderBy(x => x.GlobalIndex).ToList();
            return indicators.AsReadOnly();
        }

        public void InitMonitors()
        {
            InitMonitors(true);
        }

        internal void InitMonitors(bool calculateScore)
        {
            if (calculateScore)
            {
                CalculateScores();
            }
            _monitorPressOrder = CalculateOrder();
            _monitorPressContext = DeterminePressContext();
        }

        public PressState Press(int index)
        {
	        _logger.LogMessage("Pressed monitor {0}.", index + 1);
            if (_monitors[index].IsPressed)
            {
	            //_logger.LogMessage("Monitor with index {0} was already pressed.", index + 1);
                return PressState.AlreadyPressed;
            }
			
            if (!_monitorPressContext.IsNegativeContext)
            {
                return PressInPositiveContext(index) ? PressState.Sucess : PressState.Strike;
            }

            return PressInNegativeContext(index) ? PressState.Sucess : PressState.Strike;
        }

        public DisplayValueRecalculatonResult RecalculateGreenNeighbours(int i)
        {
	        var result = new DisplayValueRecalculatonResult();
	        var greenNonPressedConnectedMonitors = _monitors[i].GetGreenNonPressedMonitors();
	        if (greenNonPressedConnectedMonitors.Any())
	        {
		        var usedValues = GetUsedValues();
		        foreach (var monitor in greenNonPressedConnectedMonitors)
		        {
			        result.Add(monitor);
			        monitor.DisplayColor = DisplayColor.Blue;
			        monitor.DisplayValue = GetRandomValue(100, usedValues);
			        var oldScore = monitor.Score;
			        _logger.LogMessage("Monitor {0} was pressed. Which caused monitor {1} to change. New display: {2}",
				        i + 1, monitor.Index + 1, monitor.DisplayValue);
			        monitor.CalculateScore();
			        if (HasChangedSign(oldScore, monitor.Score))
			        {
				        result.HasChangedSign = true;
			        }

			        _logger.LogMessage("Score for index: {0}, changed from: {1}, to: {2}.", monitor.Index + 1, oldScore,
				        monitor.Score);
		        }

		        if (result.HasChangedSign)
		        {
			        _monitorPressContext.IsNegativeContext = IsInNegativeContext();
		        }

		        _monitorPressOrder = CalculateOrder();
	        }

	        return result;
        }

        internal DisplayValueRecalculatonResult RecalculateDisplayOnStrike(int i)
        {
	        var result = new DisplayValueRecalculatonResult();
	        var monitors = _monitors.Where(x => !x.IsPressed).ToList();
	        if (monitors.Any())
	        {
		        var usedValues = GetUsedValues();
		        foreach (var monitor in monitors)
		        {
			        result.Add(monitor);
			        monitor.DisplayValue = GetRandomValue(100, usedValues);
			        var oldScore = monitor.Score;
			        _logger.LogMessage("A strike was received. Which caused monitor {1} to change. New display: {2}", i + 1, monitor.Index + 1, monitor.DisplayValue);
			        monitor.CalculateScore();
			        if (HasChangedSign(oldScore, monitor.Score))
			        {
				        result.HasChangedSign = true;
			        }

			        _logger.LogMessage("Score for index: {0}, changed from: {1}, to: {2}.", monitor.Index + 1, oldScore, monitor.Score);
		        }
		        
		        if (result.HasChangedSign)
		        {
			        _monitorPressContext.IsNegativeContext = IsInNegativeContext();
		        }
		        
		        _monitorPressOrder = CalculateOrder();
	        }

	        return result;
        }

        internal void DumpInfo()
        {
	        StringBuilder builder = new StringBuilder();
	        builder.AppendLine("Start of error dump");
	        builder.AppendLineFormat("Monitors: \n {0}", string.Join("\n", _monitors.Select(x => x.ToString()).ToArray()));
	        builder.AppendLine("End of error dump");
	        
	        _logger.LogMessage(builder.ToString());
        }

        internal bool PressInPositiveContext(int index)
        {
            var nextMonitor = GetNextToPress();
            
            if (nextMonitor == null)
            {
	            DumpInfo();
	            throw new InvalidOperationException("Please contact Marksam on discord, this should not happend. nextMonitor is null in PressInPositiveContext()");
            }
            
            if (nextMonitor.Index == index)
            {
                //yes
                nextMonitor.IsPressed = true;
                return true;
            }
            else
            {
                //strike
                return false;
            }
        }

        internal IList<Monitor> GetMaxNegativeNonPressedValue()
        {
	        var max = _monitors.Where(x => !x.IsPressed).Max(x => x.Score);
	        return GetNonPressedWithValue(max);
        }

        internal List<int> GetUsedValues()
        {
	        return _monitors.Select(x => x.DisplayValue).Distinct().ToList();
        }
        
        internal static bool HasChangedSign(int before, int after)
        {
	        if ((before < 0 && after < 0) || (before >= 0 && after >= 0))
	        {
		        return false;
	        }

	        return ((before < 0 && after >= 0) || (before >= 0 && after < 0));
        }
        
        internal int GetRandomValue(int maxValue, List<int> usedValues)
        {
	        var value = _rnd.Range(0, maxValue);
	        while (usedValues.Contains(value))
	        {
		        value = _rnd.Range(0, maxValue);
	        }

	        usedValues.Add(value);
	        return value;
        }
        
        internal IList<Monitor> GetMinNegativeNonPressedValue()
        {
	        var min = _monitors.Where(x => !x.IsPressed).Min(x => x.Score);
	        return GetNonPressedWithValue(min);
        }

        internal IList<Monitor> GetNonPressedWithValue(int value)
        {
	        return _monitors.Where(x => x.Score == value && !x.IsPressed).ToList();
        }

        private bool TryPressMax(int index, IList<Monitor> pressCandidates)
        {
	        if (pressCandidates.Any(x => x.Index == index))
	        {
		        _monitors[index].IsPressed = true;
		        _monitorPressContext.LastPressedMax = _monitors[index];
		        _monitorPressContext.LastPressedMin = null;    
		        
		        return true;
	        }

	        return false;
        }
        
        private bool TryPressMin(int index, IList<Monitor> pressCandidates)
        {
	        if (pressCandidates.Any(x => x.Index == index))
	        {
		        _monitors[index].IsPressed = true;
		        _monitorPressContext.LastPressedMin = _monitors[index];
		        return true;
	        }

	        return false;
        }

        internal bool MaxNegativeValuesPressed()
        {
	        if (_monitorPressContext == null || _monitorPressContext.LastPressedMax == null)
	        {
		        return false;
	        }
	        
	        var lastPressedMaxMonitor = _monitorPressContext.LastPressedMax;
	        var nextPressCandidates = GetMaxNegativeNonPressedValue();
	        return !(nextPressCandidates.Max(x => x.Score) >= lastPressedMaxMonitor.Score);
        }

        internal bool PressInNegativeContext(int index)
        {
	        IList<Monitor> nextPressCandidates;
	        if (_monitorPressContext.LastPressedMax == null)
	        {
		        // if negative context and nothing previously pressed. Get the max negative value(Can be more than 1).
		        _debugPressContext.Action = DebugPressContextAction.NegativeContextNoMaxSet;
		        nextPressCandidates = GetMaxNegativeNonPressedValue();
		        return TryPressMax(index, nextPressCandidates);
	        }
	        
	        var lastPressedMaxMonitor = _monitorPressContext.LastPressedMax;
	        nextPressCandidates = GetMaxNegativeNonPressedValue();
	        if (nextPressCandidates.Max(x => x.Score) >= lastPressedMaxMonitor.Score)
	        {
		        _debugPressContext.Action = DebugPressContextAction.NegativeContextMaxPress;
		        return TryPressMax(index, nextPressCandidates); 
	        }
	        
	        // We now have to press the lowest values.
	        if (_monitorPressContext.LastPressedMin == null)
	        {
		        _debugPressContext.Action = DebugPressContextAction.NegativeContextNoMinSet;
		        nextPressCandidates = GetMinNegativeNonPressedValue();
		        return TryPressMin(index, nextPressCandidates);
	        }
	        
	        var lastPressedMinMonitor = _monitorPressContext.LastPressedMin;
	        nextPressCandidates = GetMinNegativeNonPressedValue();
	        if (nextPressCandidates.Min(x => x.Score) <= lastPressedMinMonitor.Score)
	        {
		        _debugPressContext.Action = DebugPressContextAction.NegativeContextMinPress;
		        return TryPressMin(index, nextPressCandidates);
	        }

	        return false;
        }
        
        internal Monitor GetNextToPress()
        {
	        if (IsInNegativeContext())
	        {
		        DumpInfo();
		        throw new InvalidOperationException("Please contact Marksam on discord, this should not happend. Negative context on GetNextToPress()");
	        }
	        
	        //_logger.LogMessage("Get next to press: {0}", string.Join(", ", _monitorPressOrder.Select(x => string.Format("({0}, {1})", x.Index + 1, x.IsPressed)).ToArray()));
	        var start = _monitorPressOrder.GroupBy(x => x.GetMonitorType()).Select(g => new 
                {
                    MonitorType = g.Key,
                    PressOrder = g.ToList()
                })
	            .FirstOrDefault(z => z.PressOrder.Any(zz => !zz.IsPressed));
            return start != null ? start.PressOrder.FirstOrDefault(x => !x.IsPressed) : null;
        }

        public bool IsSolved()
        {
	        if (_monitorPressContext == null)
	        {
		        return false;
	        }
	        
	        if (_monitors.All(x => x.IsPressed))
	        {
		        return true;
	        }
	        
	        if (!_monitorPressContext.IsNegativeContext)
            {
                return !_monitors.Any(x => !x.IsPressed && x.Score >= 0);
            }
	        
	        if (_monitorPressContext.LastPressedMax == null)
	        {
		        return false;
	        }

	        var pressCandidates = GetMaxNegativeNonPressedValue();
	        if (pressCandidates.Count > 0 && pressCandidates.First().Score > _monitorPressContext.LastPressedMax.Score)
	        {
		        return false;
	        }
	        
	        if (!_monitors.Where(xx => xx.Score == _monitorPressContext.LastPressedMax.Score).All(y => y.IsPressed))
	        {
		        return false;
	        }
	        
	        if (_monitorPressContext.LastPressedMin == null)
	        {
		        return false;
	        }

	        pressCandidates = GetMinNegativeNonPressedValue();
	        if (pressCandidates.Count > 0 && pressCandidates.First().Score < _monitorPressContext.LastPressedMin.Score)
	        {
		        return false;
	        }

	        return _monitors.Where(xx => xx.Score == _monitorPressContext.LastPressedMin.Score).All(y => y.IsPressed);
        }

        internal MonitorPressContext DeterminePressContext()
        {
            return new MonitorPressContext
            {
                IsNegativeContext = IsInNegativeContext()
            };
        }

        internal bool IsInNegativeContext()
        {
	        return _monitors.Where(x => !x.IsPressed).All(x => x.Score < 0);
        }

        internal void CalculateScores()
        {
            foreach(var monitor in _monitors)
            {
                monitor.CalculateScore();
            }
        }
        
        internal IList<Monitor> CalculateOrder()
        {
	        if (IsInNegativeContext())
	        {
		        var isMaxPressed = MaxNegativeValuesPressed();
		        var max = GetMaxNegativeNonPressedValue();
		        var min = GetMinNegativeNonPressedValue();

		        if (!IsSolved())
		        {
			        if (isMaxPressed)
			        {
				        _logger.LogMessage("All monitors are negative. Max values are already pressed, press monitor(s) {0} in any order.", Join(min.Select(x => (x.Index + 1).ToString())));        
			        }
			        else
			        {
				        _logger.LogMessage("All monitors are negative. First press monitor(s) {0} in any order and then monitor(s) {1} in any order.", Join(max.Select(x => (x.Index + 1).ToString())), Join(min.Select(x => (x.Index + 1).ToString())));   
			        }  
		        }
		        return new List<Monitor>();
	        }
	        else
	        {
		        var candidates = _monitors.Where(x => x.Score >= 0 && !x.IsPressed).ToList();
		        _logger.LogMessage("Candidates: " + Join(candidates.Select(x => (x.Index + 1).ToString())));

		        var order = Constants.PressingOrder[_monitors[_startIndex].MonitorColor];
		        _logger.LogMessage("The keys are: " + Join(order.Select(x => x.ToString())));

		        var monitorPressOrder = OrderBy(candidates, order);
		        _logger.LogMessage("Press order: "  + Join(monitorPressOrder.Select(x => (x.Index + 1).ToString())));
		        
		        return monitorPressOrder;
	        }
        }

        private static IList<Monitor> OrderBy(IList<Monitor> monitors, IList<MonitorType> type)
        {
            var result = new List<Monitor>();
            var monitorsGroupedByType = type.SelectMany(x => monitors.Where(y => y.GetMonitorType() == x)).GroupBy(x => x.GetMonitorType());
            foreach (var monitorGroup in monitorsGroupedByType)
            {
                result.AddRange(monitorGroup.OrderByDescending(x => x.Score).ThenByDescending(x => x.Index));
            }

            return result;
        }

        private static string Join(IEnumerable<string> values)
        {
	        return string.Join(", ", values.ToArray());
        }
    }
}

