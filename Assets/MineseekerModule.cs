using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Mineseeker;

//For the PNG generation that didn't work
/*public static class ExtensionMethod
{
    public static Texture2D toTexture2D(this RenderTexture tRex)
    {
        Texture2D tex = new Texture2D(512, 512, TextureFormat.RGB24, false);
        RenderTexture.active = tRex;
        tex.ReadPixels(new Rect(0, 0, tRex.width, tRex.height), 0, 0);
        tex.Apply();
        return tex;
    }
}*/

public class MineseekerModule : MonoBehaviour
{
    private static int _moduleIDCounter = 1;
    private int _moduleID, colorBackground;
    public KMBombInfo Info;
    public KMBombModule Module;
    public KMAudio Audio;
    public KMColorblindMode CBMode;
    public SpriteRenderer BombHolder, BombHolder2, Background;
    public Sprite[] sprites;
    public TextMesh CBText;
    public KMSelectable[] Buttons;
    public Color[] BackgroundColors;
    public string[] ColorNames, CBNames;
    private string CBCalc;
    //The color map
    //The image map
    private int[][] col = new int[][]
    {
       new int[] { 3, 7, 14, 14, 14, 11, 14, 14, 12, 13, 6, 2 },
       new int[] { 4, 6, 14, 11, 14, 3, 0, 14, 14, 5, 14, 9 },
       new int[] { 14, 1, 14, 14, 4, 13, 8, 14, 14, 7, 2, 10 },
       new int[] { 14, 14, 7, 12, 0, 1, 14, 6, 3, 1, 14, 14 },
       new int[] { 1, 8, 14, 6, 5, 14, 10, 14, 1, 0, 14, 14 },
       new int[] { 14, 9, 0, 14, 14, 13, 14, 8, 9, 14, 4, 1 },
       new int[] { 12, 13, 1, 3, 14, 14, 2, 11, 4, 14, 14, 14 },
       new int[] { 10, 14, 11, 14, 9, 6, 8, 5, 14, 14, 3, 14 },
       new int[] { 14, 14, 14, 13, 1, 5, 14, 9, 14, 14, 7, 8 },
       new int[] { 11, 14, 5, 10, 8, 14, 7, 14, 0, 2, 14, 14 },
       new int[] { 14, 2, 10, 3, 14, 14, 13, 7, 14, 14, 4, 12 },
       new int[] { 0, 14, 14, 4, 14, 14, 14, 12, 9, 10, 5, 6 }
    }, loc = new int[][]
    {
       new int[] { 5, 4, 7, 7, 7, 3, 7, 7, 0, 1, 2, 6 },
       new int[] { 6, 5, 7, 3, 7, 1, 4, 7, 7, 2, 7, 0 },
       new int[] { 7, 1, 7, 7, 2, 0, 6, 7, 7, 3, 4, 5 },
       new int[] { 7, 7, 4, 2, 1, 6, 7, 0, 3, 5, 7, 7 },
       new int[] { 4, 2, 7, 0, 3, 7, 1, 7, 5, 6, 7, 7 },
       new int[] { 7, 0, 6, 7, 7, 5, 7, 2, 1, 7, 3, 4 },
       new int[] { 2, 3, 1, 5, 7, 7, 0, 6, 4, 7, 7, 7 },
       new int[] { 0, 7, 5, 7, 6, 2, 3, 4, 7, 7, 1, 7 },
       new int[] { 7, 7, 7, 5, 0, 4, 7, 1, 7, 7, 6, 3 },
       new int[] { 1, 7, 3, 6, 4, 7, 5, 7, 2, 0, 7, 7 },
       new int[] { 7, 6, 0, 4, 7, 7, 2, 3, 7, 7, 5, 1 },
       new int[] { 3, 7, 7, 1, 7, 7, 7, 5, 6, 4, 0, 2 }
    };
    //The color table
    private int[,] tab = new int[,]
    {
        //White
        { 6, 3, 0, 1, 2, 6, 0 },
        //Grey
        { 0, 4, 4, 1, 1, 2, 3 },
        //Pink
        { 1, 5, 3, 2, 3, 6, 5 },
        //Red
        { 4, 2, 2, 1, 6, 1, 4 },
        //Brick
        { 1, 5, 6, 3, 0, 3, 2 },
        //Brown
        { 2, 0, 4, 6, 4, 2, 5 },
        //Orange
        { 0, 3, 2, 0, 5, 5, 6 },
        //Yellow
        { 5, 6, 3, 2, 2, 5, 4 },
        //Lime
        { 3, 2, 6, 1, 4, 3, 1 },
        //Forest
        { 0, 0, 1, 3, 2, 6, 6 },
        //Cyan
        { 5, 4, 4, 0, 5, 6, 4 },
        //Blue
        { 0, 5, 6, 3, 5, 0, 4 },
        //Lavender
        { 4, 1, 3, 5, 2, 3, 0 },
        //Purple
        { 5, 6, 1, 1, 4, 1, 0 }
    };
    //Walls map
    private string[,] dir = new string[,]
    {
        { "R", "LR",  "LR",  "LR",  "LR",  "LDR",  "LR",  "L",  "D",  "R", "LR", "LD" },
        { "D", "DR", "LDR", "LDR", "L", "DU", "DR", "LR", "LDRU", "LDR", "L", "DU" },
        { "DU", "DRU", "LDRU", "LU", "R", "LDU", "DU", "D", "RU", "LDU", "D", "DU" },
        { "DU", "DRU", "LU", "R", "LD", "U", "DU", "DRU", "LD", "U", "DU", "DU" },
        { "DU", "DU", "R", "LD", "RU", "LD", "U", "DRU", "LDRU", "LR", "LU", "DU" },
        { "DU", "RU", "L", "DRU", "LD", "DU", "R", "LRU", "LU", "R", "LDR", "LDU" },
        { "DRU", "LDR", "LD", "RU", "LDU", "RU", "LR", "LD", "R", "LD", "RU", "LU" },
        { "DRU", "LRU", "LU", "D", "RU", "LDR", "LD", "RU", "LD", "RU", "LDR", "LD" },
        { "U", "DR", "LR", "LU", "D", "RU", "LRU", "L", "RU", "L", "DRU", "LDU" },
        { "DR", "LDU", "R", "LR", "LDU", "DR", "LR", "LD", "R", "LR", "LDRU", "LU" },
        { "DRU", "LDRU", "LR", "L", "DU", "U", "D", "RU", "LDR", "LD", "U", "D" },
        { "RU", "LU", "R", "LR", "LRU", "LR", "LRU", "L", "RU", "LRU", "LR", "LU" }
    };
    //All possible solutions for the current quadrant, for logging purposes.
    //The arrays are based on each image and their current locations.
    private int[][] Destinations = new int[][] { new[]{ 0, 0 }, new[] { 0, 0 }, new[] { 0, 0 }, new[] { 0, 0 }, new[] { 0, 0 }, new[] { 0, 0 }, new[] { 0, 0 } };
    //Colorblind text locations. They're kind of hard to center.
    private float[][] Translations = new float[][] { new[] { -.112f, .02f }, new[] { -.08f, .02f }, new[] { -.12f, 0 }, new[] { -.1f, 0 }, new[] { -.1f, -.02f }, new[] { -.09f, .01f }, new[] { -.1f, -.01f } };
    //The index value for the starting bomb | The value for the destination bomb, before Two Factor are factored in |
    //The index value for the destination bomb | The Two Factor sum | a/d: The current visable location.
    private int start, startingValue, destination, tF, a, d;
    private char[] vowels = { 'A', 'E', 'I', 'O', 'U' };
    private int[] serial, startingLocation;
    private Queue<IEnumerable> queue = new Queue<IEnumerable>();
    //Queue for keeping track of all destination possibilites for logging purposes
    private Queue<int[]> Movement = new Queue<int[]>();
    //The list of possible destination values for logging purposes
    private List<int[]> map = new List<int[]>();
    //Keep track of when the module is processing an input (don't process any others while ready is false) |
    //Don't allow interactions when !_isActive or solved | colorblind boolean variable for TP compatibility
    private bool ready = true, _isActive = false, solved = false, cb = false;

    // Use this for initialization
    void Start()
    {
	    cb = CBMode.ColorblindModeActive;
        if (!cb) CBText.gameObject.SetActive(false);
        _moduleID = _moduleIDCounter++;
        //This is PNG generation code that doesn't work at all. Keeping it here in case a PNG reader is ever added to the Logfile Analyzer.
        /*Debug.LogFormat("[Mineseeker #{0}] " + Convert.ToBase64String(sprites[2].texture.GetRawTextureData()));
        RenderTexture copy = new RenderTexture(sprites[2].texture.width, sprites[2].texture.height, 0);
        Graphics.Blit(sprites[2].texture, copy);
        Texture2D png = copy.toTexture2D();
        Debug.LogFormat("[Mineseeker #{0}] " + Convert.ToBase64String(png.EncodeToPNG()), _moduleID);
        Debug.LogFormat("[Mineseeker #{0}] " + png.format, _moduleID);*/
        Background.color = Color.black;
        startingValue += Info.GetBatteryHolderCount() + Info.GetPortPlateCount();
        var b = Info.GetIndicators().SelectMany((x) => x.ToUpperInvariant().ToCharArray());
        foreach (char c in b)
        {
            if (vowels.Any(x => x == c)) startingValue--;
            else startingValue++;
        }
        serial = Info.GetSerialNumberNumbers().ToArray();
        start = UnityEngine.Random.Range(0, sprites.Length);
        colorBackground = UnityEngine.Random.Range(0, BackgroundColors.Count());
        var selected = false;
        while (!selected)
        {
            for (int i = 0; i < loc.Length; i++)
            {
                if (col[i].Contains(colorBackground) && loc[i].Contains(start))
                {
                    var z = Array.IndexOf(loc[i], start);
                    var y = Array.IndexOf(col[i], colorBackground);
                    if (z == y)
                    {
                        selected = true;
                        a = i;
                        d = y;
                    }
                }
            }
            if (!selected)
            {
                start = UnityEngine.Random.Range(0, sprites.Length);
                colorBackground = UnityEngine.Random.Range(0, BackgroundColors.Count());
            }
        }
        DestinationValues();
        BombHolder.sprite = sprites[start];
        for (int i = 0; i < Buttons.Length; i++)
        {
            int j = i;
            Buttons[i].OnInteract = ButtonHandler(j);
        }
        startingLocation = new[] { a, d };
        Module.OnActivate += delegate ()
        {
            Background.color = BackgroundColors[colorBackground];
            Debug.LogFormat("[Mineseeker #{0}] Color is {1}", _moduleID, ColorNames[Array.IndexOf(BackgroundColors, Background.color)]);
            Debug.LogFormat("[Mineseeker #{0}] Bomb shown is {1}", _moduleID, sprites[start].name);
            Debug.LogFormat("[Mineseeker #{0}] Current location is [{2}, {1}]", _moduleID, a + 1, d + 1);
            CBText.text = ((Char)(65 + a)).ToString() + CBNames[colorBackground];
            CBText.transform.localPosition = new Vector3(Translations[start][0], 0.55f, Translations[start][1]);
            Calculate();
            _isActive = true;
        };
        StartCoroutine(WaitForInput());
    }

    void DestinationValues()
    {
        var set = false;
        Destinations[start] = new[] { a, d };
        var y = a;
        var z = d;
        while (!set)
        {
            if (loc[y][z] != 7) Destinations[loc[y][z]] = new[] { y, z };
            foreach (char c in dir[y, z])
            {
                switch(c)
                {
                    case 'L':
                        if (!map.Any(x => x.SequenceEqual(new int[] { y, z - 1 })))
                        {
                            map.Add(new int[] { y, z - 1 });
                            Movement.Enqueue(new int[] { y, z - 1 });
                        }
                        break;
                    case 'D':
                        if (!map.Any(x => x.SequenceEqual(new int[] { y + 1, z })))
                        {
                            map.Add(new int[] { y + 1, z });
                            Movement.Enqueue(new int[] { y + 1, z });
                        }
                        break;
                    case 'R':
                        if (!map.Any(x => x.SequenceEqual(new int[] { y, z + 1 })))
                        {
                            map.Add(new int[] { y, z + 1 });
                            Movement.Enqueue(new int[] { y, z + 1 });
                        }
                        break;
                    case 'U':
                        if (!map.Any(x => x.SequenceEqual(new int[] { y - 1, z })))
                        {
                            map.Add(new int[] { y - 1, z });
                            Movement.Enqueue(new int[] { y - 1, z });
                        }
                        break;
                }
            }
            if (Movement.Count == 0) set = true;
            else
            {
                var coordinates = Movement.Dequeue();
                y = coordinates[0];
                z = coordinates[1];
            }
        }
    }

    void Calculate()
    {
        destination = startingValue;
        tF = Info.GetTwoFactorCodes().Sum(x => x % 10);
        destination -= tF;
        while (serial.Contains(destination))
        {
            destination--;
        }
        if (destination < 0) destination = serial[0];
        while (destination > 6) destination -= 7;
        Debug.LogFormat("[Mineseeker #{0}] Number is {1}", _moduleID, destination);
        Debug.LogFormat("[Mineseeker #{0}] Desired Bomb is {1}", _moduleID, sprites[tab[colorBackground, destination]].name);
        Debug.LogFormat("[Mineseeker #{0}] Destination is [{2}, {1}]", _moduleID, Destinations[tab[colorBackground, destination]][0] + 1, Destinations[tab[colorBackground,destination]][1] + 1);
    }

    KMSelectable.OnInteractHandler ButtonHandler(int i)
    {
        return delegate ()
        {
            if (!_isActive || solved) return false;
            Buttons[i].AddInteractionPunch(0.5f);
            Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
            queue.Enqueue(ButtonPress(i));
            return false;
        };
    }

    private IEnumerator WaitForInput()
    {
        do
        {
            yield return null;
            if (queue.Count > 0)
            {
                IEnumerable press = queue.Dequeue();
                foreach (object item in press) yield return item;
            }
        }
        while (ready);
    }

    IEnumerable ButtonPress(int i)
    {
        var oP = BombHolder.transform.localPosition;
        var cP = new int[] { a, d };
        var rP = BombHolder.transform.localPosition;
        var move = "";
        ready = false;
        var strike = true;
        switch (i)
        {
            case 0:
                move = "top";
                oP.z -= 1.1f;
                rP.z += 1.1f;
                if (dir[a, d].Contains('U'))
                {
                    a--;
                    strike = false;
                }
                break;
            case 1:
                move = "right";
                oP.x -= 1.1f;
                rP.x += 1.1f;
                if (dir[a, d].Contains('R'))
                {
                    d++;
                    strike = false;
                }
                break;
            case 2:
                move = "bottom";
                oP.z += 1.1f;
                rP.z -= 1.1f;
                if (dir[a, d].Contains('D'))
                {
                    a++;
                    strike = false;
                }
                break;
            case 3:
                move = "left";
                oP.x += 1.1f;
                rP.x -= 1.1f;
                if (dir[a, d].Contains('L'))
                {
                    d--;
                    strike = false;
                }
                break;
            case 4:
                if (BombHolder.sprite == sprites[tab[colorBackground, destination]])
                {
                    solved = true;
                    Module.HandlePass();
                }
                else
                {
                    Module.HandleStrike();
                    if (!BombHolder.sprite.Equals(sprites[7])) Debug.LogFormat("[Mineseeker #{0}] Incorrect bomb chosen.", _moduleID);
                    else Debug.LogFormat("[Mineseeker #{0}] Coordinate {2},{1} does not contain a bomb.", _moduleID, a + 1, d + 1);
                    ready = true;
                }
                break;
        }
        if (i == 4) yield break;
        yield return MoveScreen(oP, rP, cP, move, strike);
        yield return null;
    }

    IEnumerator MoveScreen(Vector3 bh1, Vector3 bh2O, int[] oP, string m, bool strike = false)
    {
        var t = 0.0f;
        var duration = 0.25f;
        BombHolder2.transform.localPosition = bh2O;
        BombHolder2.sprite = sprites[7];
        var b = BombHolder.transform.localPosition;
        if (CBText.gameObject.activeSelf) CBText.gameObject.SetActive(false);
        if (strike)
        {
            duration /= 2;
            bh1.x /= 2;
            bh1.z /= 2;
            bh2O.x /= 2;
            bh2O.z /= 2;
            while (t < duration)
            {
                yield return null;
                t = Mathf.Min(t + Time.deltaTime, duration);
                if (oP.SequenceEqual(startingLocation)) Background.transform.localPosition = Vector3.Lerp(b, bh1, Mathf.SmoothStep(0.0f, 1.0f, t / duration));
                BombHolder.transform.localPosition = Vector3.Lerp(b, bh1, Mathf.SmoothStep(0.0f, 1.0f, t / duration));
                BombHolder2.transform.localPosition = Vector3.Lerp(bh2O, b, Mathf.SmoothStep(0.0f, 1.0f, t / duration));
            }
            Module.HandleStrike();
            //Clear the queue for TP Compatibility
            queue.Clear();
            Debug.LogFormat("[Mineseeker #{0}] Wall detected to the {3} at coordinate [{2},{1}]", _moduleID, a + 1, d + 1, m);
            t = 0;
            while (t < duration)
            {
                yield return null;
                t = Mathf.Min(t + Time.deltaTime, duration);
                if (oP.SequenceEqual(startingLocation))
                {
                    Background.transform.localPosition = Vector3.Lerp(bh1, b, Mathf.SmoothStep(0.0f, 1.0f, t / duration));
                }
                BombHolder.transform.localPosition = Vector3.Lerp(bh1, b, Mathf.SmoothStep(0.0f, 1.0f, t / duration));
                BombHolder2.transform.localPosition = Vector3.Lerp(b, bh2O, Mathf.SmoothStep(0.0f, 1.0f, t / duration));
            }
            if (cb && oP.SequenceEqual(startingLocation)) CBText.gameObject.SetActive(true);
            ready = true;
            yield break;
        }
        var move = m;
        switch (m)
        {
            case "top":
                move = "up";
                break;
            case "bottom":
                move = "down";
                break;
        }
        BombHolder2.sprite = sprites[loc[a][d]];
        while (t < duration)
        {
            yield return null;
            t = Mathf.Min(t + Time.deltaTime, duration);
            if (oP.SequenceEqual(startingLocation)) Background.transform.localPosition = Vector3.Lerp(b, bh1, Mathf.SmoothStep(0.0f, 1.0f, t / duration));
            else if (new int[] { a, d }.SequenceEqual(startingLocation)) Background.transform.localPosition = Vector3.Lerp(bh2O, b, Mathf.SmoothStep(0.0f, 1.0f, t / duration));
            BombHolder.transform.localPosition = Vector3.Lerp(b, bh1, Mathf.SmoothStep(0.0f, 1.0f, t / duration));
            BombHolder2.transform.localPosition = Vector3.Lerp(bh2O, b, Mathf.SmoothStep(0.0f, 1.0f, t / duration));
        }
        if (cb && new int[] { a, d }.SequenceEqual(startingLocation)) CBText.gameObject.SetActive(true);
        Debug.LogFormat("[Mineseeker #{0}] Moved {1} from coordinate [{3},{2}] to [{5},{4}]", _moduleID, move, oP[0] + 1, oP[1] + 1, a + 1, d + 1);
        BombHolder.sprite = sprites[loc[a][d]];
        BombHolder.transform.localPosition = new Vector3(0, 0.55f, 0);
        BombHolder2.transform.localPosition = bh2O;
        yield return null;
        ready = true;
    }

    private string TwitchHelpMessage = "Interact with the module using !{0} udlr NSEW, and use !{0} submit to submit your selection. Toggle colorblind mode using !{0} colorblind.";

    private IEnumerator ProcessTwitchCommand(string input)
    {
        input = input.ToLowerInvariant();
        var submit = false;
        var presses = new List<KMSelectable>();
	    if (input.Equals("colorblind"))
	    {
            yield return null;
	        cb = !cb;
	        if (new[] {a, d}.SequenceEqual(startingLocation) && cb) CBText.gameObject.SetActive(true);
            else if (CBText.gameObject.activeSelf) CBText.gameObject.SetActive(false);
            yield break;
	    }	
        if (input.EndsWith("submit") || input.StartsWith("submit"))
        {
            submit = true;
            input = input.Replace("submit", "");
        }
        input = input.Replace("press", "");
        foreach (char c in input)
        {
            switch (c)
            {
                case 'u':
                case 'n':
                    presses.Add(Buttons[0]);
                    break;
                case 'r':
                case 'e':
                    presses.Add(Buttons[1]);
                    break;
                case 'd':
                case 's':
                    presses.Add(Buttons[2]);
                    break;
                case 'l':
                case 'w':
                    presses.Add(Buttons[3]);
                    break;
                case ' ':
                    break;
                default:
                    yield break;
            }
        }
        if (submit) presses.Add(Buttons[4]);
        yield return null;
        yield return presses.ToArray();
        //Focus on the module until the queue is empty
        //This is so solves and strikes are detected properly
        yield return new WaitUntil(() => queue.Count == 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (tF != Info.GetTwoFactorCodes().Sum(x => x % 10)) Calculate();
    }
}
