using UnityEngine;
using ConnectedMonitors;
using System.Collections.Generic;
using System.Linq;
using System;
using Color = ConnectedMonitors.Color;
using System.Collections;
using System.Text.RegularExpressions;

/*
 Some variables may be internal for testing purposes.
*/
public class ConnectedMonitorsScript : MonoBehaviour 
{
    public KMBombModule Module;
    public KMBombInfo Info;
    public KMAudio Audio;

    //Monitors
    public KMSelectable[] MonitorButtons;
    public AudioClip[] ButtonSounds;

    //Screens
    public MeshRenderer[] ScreenRenderers;
    public Material[] DisplayColors;
    public Material[] ScreenColors;
    public TextMesh[] MonitorTexts;

    //Indicators
    public GameObject[] IndicatorsObject;
    public Material[] IndicatorColors;
    public Light[] Lights;
    public UnityEngine.Color[] LightColors;
    public Material BlackMat;

    //Cables
    public MeshRenderer[] CableRenderers;
    public Material[] CableColors;

    private readonly List<Cable> _cables = new List<Cable>();
    private readonly List<Monitor> _monitors = new List<Monitor>();
    private IList<Monitor> _monitorPressOrder;

    private ConnectedMonitorsLogger _connectedMonitorsLogger;
    private ConnectedMonitorsSolver _solver;

    private static int _moduleIdCounter = 1;
    private int _moduleId = 0;
    private static int _connectedCounter = 1;
    private int _connectedCount;
    
    
    private bool isSolved = false;
    private static readonly IConnectedMonitorsRandom rnd = new ConnectedMonitorsUnityRandom();
    private static readonly Regex TwitchPlaysRegex = new Regex("^press (([1-9]|1[012345])( (1[012345]|[1-9])){0,14})$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private static readonly Regex CableRegex = new Regex("^cable ([1-9]|1[012345]) (1[012345]|[1-9])$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    private static readonly List<int> cycle = new List<int>{0, 1, 2, 6, 10, 14, 13, 12, 11, 7, 3, 4, 5, 9, 8}; 

    // Use this for initialization
    void Start()
    {
        _moduleId = _moduleIdCounter++;
        _connectedCount = _connectedCounter++;
        _connectedMonitorsLogger = new ConnectedMonitorsLogger(_moduleId);
        float scalar = transform.lossyScale.x;
        foreach (var light in Lights)
        {
            light.range *= scalar;
        }
        InitMonitors();
        InitIndicators();
        InitCables();
		
        _solver = new ConnectedMonitorsSolver(_connectedMonitorsLogger, _monitors, rnd);

        _solver.InitMonitors();
        
        //_connectedMonitorsLogger.LogMessage(PrintScores());
        Module.OnActivate += Activate;
        
    }

    private void Activate()
    {
        if (_connectedCount == 1)
        {
            StartCoroutine(StartSound());
        }
        
        foreach (var cr in CableRenderers)
        {
            var cable = _cables.Find(x => x.Tag.Equals(cr.name, StringComparison.InvariantCultureIgnoreCase));
            cr.material = CableColorToMaterial(cable.Color);
        }

        foreach (var indicator in _solver.GetAllIndicators())
        {
            IndicatorsObject[indicator.GlobalIndex].SetActive(true);
            IndicatorsObject[indicator.GlobalIndex].GetComponent<MeshRenderer>().material = IndicatorColorToMaterial(indicator.Color);
            if (indicator.Color != Color.Orange)
            {
                Lights[indicator.GlobalIndex].intensity = 30;
            }
            Lights[indicator.GlobalIndex].color = LightColors[(int)indicator.Color];
        }

        for (var i = 0; i < 15; ++i)
        {
            var materials = ScreenRenderers[i].materials;
            materials[1] = ScreenColors[(int)_monitors[i].MonitorColor];
            materials[0] = DisplayColors[(int)_monitors[i].DisplayColor];
            ScreenRenderers[i].materials = materials;
            MonitorTexts[i].text = _monitors[i].DisplayValue.ToString();
        }
        StartCoroutine(BlinkLights());
        for (int i = 0; i < MonitorButtons.Length; i++)
        {
            var j = i;
            MonitorButtons[j].OnInteract += delegate
            {
                MonitorButtons[j].AddInteractionPunch(.5f);
                Audio.PlaySoundAtTransform(ButtonSounds[rnd.Range(0, ButtonSounds.Length)].name, Module.transform);
                if (isSolved)
                    return false;
                HandlePress(j);
                return false;
            };
        }
    }

    // private string PrintScores()
    // {
    //     var separator = string.Empty;
    //     var sb = new StringBuilder();
    //     sb.Append("(");
    //     foreach (var item in _monitors)
    //     {
    //         sb.Append(separator).Append(string.Format("({0}, {1})", item.Index + 1, item.Score));
    //         if (separator.Length == 0)
    //         {
    //             separator = ", ";
    //         }
    //     }
    //     sb.Append(")");
    //     return sb.ToString();
    // }

    private void HandlePress(int i)
    {
        var result = _solver.Press(i);
        if (result == PressState.AlreadyPressed)
        {
            return;
        }
        if (result == PressState.Strike)
        {
            _connectedMonitorsLogger.LogMessage("That is incorrect. Strike!");
            var changes = _solver.RecalculateDisplayOnStrike(i);
            if (changes.Changed.Any())
            {
                foreach (var change in changes.Changed)
                {
                    MonitorTexts[change.Index].text = change.DisplayValue.ToString();
                }
            }
            Module.HandleStrike();
        }
        else
        {
            MonitorTexts[i].text = "✓";
            var changes = _solver.RecalculateGreenNeighbours(i);
            if (changes.Changed.Any())
            {
                //set green connected monitors to blue
                foreach (var change in changes.Changed)
                {
                    var materials = ScreenRenderers[change.Index].materials;
                    materials[0] = DisplayColors[0];
                    ScreenRenderers[change.Index].materials = materials;
                    MonitorTexts[change.Index].text = change.DisplayValue.ToString();
                }
            }
            if (_solver.IsSolved())
            {
                isSolved = true;
                _connectedMonitorsLogger.LogMessage("That is correct, module solved!");
                StartCoroutine(SolveAnimation());
            }
        }
    }

    private void InitMonitors()
    {
        var usedValues = new List<int>();
        var displayColors = GetDisplayColors();
        for(var i = 0; i < 15; ++i)
        {
            var monitor = new Monitor(i, _connectedMonitorsLogger)
            {
                DisplayValue = GetRandomValue(100, usedValues),
                MonitorColor = GetColor(rnd.Range(0, 5)),
                DisplayColor = displayColors[i]
            };

            _monitors.Add(monitor);
        }
    }

    private void InitCables()
    {
        foreach (var from in Constants.CableStructures.Keys)
        {
            foreach (var to in Constants.CableStructures[from])
            {
                var cable = new Cable
                {
                    Color = GetRandomColor(),
                    From = _monitors[from],
                    To = _monitors[to.Index],
                    Direction = to.Direction
                };

                var fromMonitor = _monitors[from];
                var toMonitor = _monitors[to.Index];
				
                fromMonitor.AddOutCable(cable);
                toMonitor.AddInCable(cable);
                _connectedMonitorsLogger.LogMessage("Cable going from monitor: {0} To monitor: {1} is {2}.", fromMonitor.Index + 1, toMonitor.Index + 1, cable.Color.ToString());
                _cables.Add(cable);
            }
        }
    }

    private static List<DisplayColor> GetDisplayColors()
    {
        var usedValues = new List<int>();
        var displayColors = Enumerable.Repeat(DisplayColor.Blue, 15).ToList();
        var noOfGreen = rnd.Range(1, 4);

        for(var i = 0; i < noOfGreen; ++i)
        {
            displayColors[GetRandomValue(15, usedValues)] = DisplayColor.Green;
        }

        return displayColors;
    }

    private static Color GetRandomColor()
    {
        return GetColor(rnd.Range(0, 6));
    }

    private static int GetRandomValue(int maxValue, List<int> usedValues)
    {
        var value = rnd.Range(0, maxValue);
        while (usedValues.Contains(value))
        {
            value = rnd.Range(0, maxValue);
        }

        usedValues.Add(value);
        return value;
    }

    private void InitIndicators()
    {
        var globalIndex = 0;
        for (int i = 0; i < 15; ++i)
        {
            var monitor = i;
            var indicators = new List<IndicatorInfo>();	
            for(int x = 0; x < 3; ++x)
            {
                indicators.Add(new IndicatorInfo
                {
                    Enabled = false,
                    Monitor = monitor,
                    Index = x,
                    Color = GetColor(rnd.Range(0, 6)),
                    GlobalIndex = globalIndex++,
                    IsFlashing = IsFlashing()
                });
            }
            var activeIndicators = rnd.Range(0, 4);
            for(int z = 0; z < activeIndicators; ++z)
            {
                indicators[z].Enabled = true;
            }

            var indicatorsToAdd = indicators.Where(x => x.Enabled).ToList();
            _monitors[i].AddIndicators(indicatorsToAdd);
        }
    }

    private static Color GetColor(int value)
    {
        switch (value)
        {
            case 0:
                return Color.Red;
            case 1:
                return Color.Orange;
            case 2:
                return Color.Green;
            case 3:
                return Color.Blue;
            case 4:
                return Color.Purple;
            default:
                return Color.White;
        }
    }

    private Material IndicatorColorToMaterial(Color color)
    {
        return IndicatorColors[(int)color];
    }
    private Material CableColorToMaterial(Color color)
    {
        return CableColors[(int)color];
    }

    private static bool IsFlashing()
    {
        return rnd.Range(0, 6) == 1;
    }

    private IEnumerator StartSound()
    {
        Audio.PlaySoundAtTransform(ButtonSounds[5].name, Module.transform);
        yield return new WaitForSeconds(.2f);
        Audio.PlaySoundAtTransform(ButtonSounds[5].name, Module.transform);
        yield return new WaitForSeconds(.2f);
        Audio.PlaySoundAtTransform(ButtonSounds[6].name, Module.transform);
        yield return new WaitForSeconds(.4f);
        Audio.PlaySoundAtTransform(ButtonSounds[5].name, Module.transform);
        yield return new WaitForSeconds(.4f);
        Audio.PlaySoundAtTransform(ButtonSounds[7].name, Module.transform);
    }
	
    private IEnumerator BlinkLights()
    {
        var indicators = _solver.GetAllIndicators().Where(x => x.IsFlashing).ToList();
        while (!isSolved)
        {
            yield return new WaitForSeconds(1f);
            foreach(var indicator in indicators)
            {
                Lights[indicator.GlobalIndex].enabled = false;
                IndicatorsObject[indicator.GlobalIndex].GetComponent<MeshRenderer>().material = BlackMat;
            }
            yield return new WaitForSeconds(1f);
            foreach (var indicator in indicators)
            {
                Lights[indicator.GlobalIndex].enabled = true;
                IndicatorsObject[indicator.GlobalIndex].GetComponent<MeshRenderer>().material = IndicatorColorToMaterial(indicator.Color);
            }
        }

        foreach (var indicator in _solver.GetAllIndicators())
        {
            Lights[indicator.GlobalIndex].enabled = false;
            IndicatorsObject[indicator.GlobalIndex].GetComponent<MeshRenderer>().material = BlackMat;
        }
    }

    private IEnumerator SolveAnimation()
    {
        foreach (var monitor in _monitors)
        {
            var materials = ScreenRenderers[monitor.Index].materials;
            materials[1] = ScreenColors[(int)monitor.MonitorColor];    
            materials[0] = DisplayColors[0];
            ScreenRenderers[monitor.Index].materials = materials;
            MonitorTexts[monitor.Index].text = string.Empty;
        }

        for (var x = 0; x < _monitors.Count; x++)
        {
            for (var i = 0; i < (_monitors.Count-x); i++)
            {
                Material[] materials;
                if (i != 0)
                {
                    materials = ScreenRenderers[cycle[i - 1]].materials;
                    materials[1] = ScreenColors[(int)_monitors[cycle[i - 1]].MonitorColor];
                    materials[0] = DisplayColors[0];
                    ScreenRenderers[cycle[i - 1]].materials = materials;
                }

                materials = ScreenRenderers[cycle[i]].materials;
                materials[1] = ScreenColors[(int)_monitors[cycle[i]].MonitorColor];
                materials[0] = DisplayColors[1];
                ScreenRenderers[cycle[i]].materials = materials;
                yield return new WaitForSeconds(.05f);
            }
            Audio.PlaySoundAtTransform(ButtonSounds[rnd.Range(0, ButtonSounds.Length)].name, Module.transform);
        }
        yield return new WaitForSeconds(.7f);
        foreach (var monitor in MonitorTexts)
        {
            monitor.text = "✓";
        }
        Audio.PlaySoundAtTransform(ButtonSounds[7].name, Module.transform);
        Module.HandlePass();
    }

#pragma warning disable 414
    private const string TwitchHelpMessage = "Press monitor(s) in a specific order using !{0} press 1 2 3 4 5. (Maximum 15 inputs)(Reading order).";
#pragma warning restore 414

    public IEnumerator ProcessTwitchCommand(string command)
    {
        command = command.ToLowerInvariant().Trim();
        var match = TwitchPlaysRegex.Match(command);

        if (match.Success)
        {
            var tokenized = match.Groups[1].ToString().Split(' ');
            if(!tokenized.All(new HashSet<string>().Add))
            {
                yield return "sendtochaterror No duplicate numbers allowed in a command";
                yield break;
            }
            
            yield return null;
            for (var i = 0; i < tokenized.Length; ++i)
            {
                MonitorButtons[int.Parse(tokenized[i])-1].OnInteract();
                yield return new WaitForSeconds(.35f);
            }

            if (isSolved)
            {
                yield return "solve";
            }
        }
        yield break;
    } 
}

public class CableInfo
{
    public CableInfo(int index, CableDirection direction)
    {
        Index = index;
        Direction = direction;
    }

    public int Index { get; private set; }

    public CableDirection Direction { get; private set; }
}