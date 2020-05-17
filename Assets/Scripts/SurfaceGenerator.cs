using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class SurfaceGenerator : MonoBehaviour
{
    const float pi = Mathf.PI;
    const float e = 2.71828F;

    public Transform pointPrefab;
    public Text function;

    [Range(10, 1000)]
    public int resolution = 200;
    public int rangeMin = -10;
    public int rangeMax = 10;

    public float gravityScale = 0.5f;

    Transform[] points;

    void Awake()
    {
        QualitySettings.shadows = ShadowQuality.Disable;
        float step = (float)(rangeMax - rangeMin) / resolution;
        step /= 2;
        Vector3 scale = Vector3.one * step;
        points = new Transform[resolution * resolution];
        for (int i = 0; i < points.Length; i++)
        {
            Transform point = Instantiate(pointPrefab);
            point.localScale = scale;
            point.SetParent(transform, false);
            points[i] = point;
        }
    }

    public void generate()
    {
        Physics.gravity = new Vector3(0, -gravityScale, 0);
        float step = (float)(rangeMax - rangeMin) / resolution;
        string f = function.text;
        Debug.Log("Function: " + f);
        f = simplify(f);
        Debug.Log("Simplified function: " + f);
        f = parse(f);
        Debug.Log("Parsed function: " + f);
        for (int i = 0, z = 0; z < resolution; z++)
        {
            float v = (z + 0.5f) * step - 1f;
            for (int x = 0; x < resolution; x++, i++)
            {
                float u = (x + 0.5f) * step - 1f;
                points[i].localPosition = surfaceFunction(u, v, f);
            }
        }
    }

    private static Vector3 surfaceFunction(float u, float v, string f)
    {
        Vector3 p;
        p.x = u;
        p.z = v;
        p.y = evaluateString(u, v, f);
        return p;
    }

    private static string simplify(string f)
    {
        for (int i = 0; i < f.Length; i++)
        {
            if (f[i].Equals('e'))
            {
                if (i < f.Length - 1 && f[i+1].Equals('x'))
                {
                    f = f.Substring(0, i + 1) + f.Substring(i + 3, f.Length - i - 3);
                }
                else
                {
                    System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder(f);
                    stringBuilder[i] = 'E';
                    f = stringBuilder.ToString();
                }
            }
            if (f[i].Equals('l'))
            {
                f = f.Substring(0, i + 1) + f.Substring(i + 3, f.Length - i - 3);
            }
            if (f[i].Equals('p'))
            {
                f = f.Substring(0, i + 1) + f.Substring(i + 2, f.Length - i - 2);
            }
            if (f[i].Equals('s'))
            {
                f = f.Substring(0, i + 1) + f.Substring(i + 3, f.Length - i - 3);
            }
            if (f[i].Equals('c'))
            {
                f = f.Substring(0, i + 1) + f.Substring(i + 3, f.Length - i - 3);
            }
        }
        return f;
    }

    private static string parse(string f)
    {
        if (f.Length == 0) return "";
        for (int i = 0; i < f.Length; i++)
        {
            if (f[i].Equals('l') || f[i].Equals('e'))
            {
                int op = 0, comma = 0;
                string a = "", b = "";
                for (int j = i + 1; j < f.Length; j++)
                {
                    if (f[i].Equals('l') || f[i].Equals('e'))
                    {
                        string suffix = parse(f.Substring(j, f.Length - j));
                        f = f.Substring(0, j) + suffix;
                        j += suffix.Length - 1;
                    }
                    if (f[j] == '(') op++;
                    if (f[j] == ')') op--;
                    if (f[j] == ',')
                    {
                        a = f.Substring(i + 2, j - i - 2);
                        comma = j;
                    }
                    if (op == 0)
                    {
                        b = f.Substring(comma + 1, j - comma - 1);
                        if (f[i].Equals('l'))
                            f = f.Substring(0, i) + "(" + a + "l" + b + f.Substring(j, f.Length - j);
                        else
                            f = f.Substring(0, i) + "(" + a + "e" + b + f.Substring(j, f.Length - j);
                        break;
                    }
                }
                break;
            }
        }
        return f;
    }

    private static float evaluateString(float u, float v, string f)
    {
        Stack<float> valueStack = new Stack<float>();
        Stack<char> operationStack = new Stack<char>();
        for (int i = 0; i < f.Length; i++)
        {
            if (char.IsDigit(f[i]))
            {
                int startIndex = i;
                while (char.IsDigit(f[i]) || f[i].Equals('.'))
                {
                    i++;
                    continue;
                }
                valueStack.Push(float.Parse(f.Substring(startIndex, i - startIndex)));
            }
            else if (f[i].Equals('p'))
            {
                valueStack.Push(pi);
            }
            else if (f[i].Equals('E'))
            {
                valueStack.Push(e);
            }
            else if (f[i].Equals('x'))
            {
                valueStack.Push(u);
            }
            else if (f[i].Equals('z'))
            {
                valueStack.Push(v);
            }
            else if (!f[i].Equals('(') && !f[i].Equals(')'))
            {
                operationStack.Push(f[i]);
            }
            else if (f[i].Equals(')'))
            {
                char operation = operationStack.Pop();
                if (operation.Equals('s'))
                {
                    float a = valueStack.Pop();
                    valueStack.Push(Mathf.Sin(a));
                }
                if (operation.Equals('c'))
                {
                    float a = valueStack.Pop();
                    valueStack.Push(Mathf.Cos(a));
                }
                if (operation.Equals('+'))
                {
                    float b = valueStack.Pop();
                    float a = valueStack.Pop();
                    valueStack.Push(a + b);
                }
                if (operation.Equals('-'))
                {
                    float b = valueStack.Pop();
                    float a = valueStack.Pop();
                    valueStack.Push(a - b);
                }
                if (operation.Equals('*'))
                {
                    float b = valueStack.Pop();
                    float a = valueStack.Pop();
                    valueStack.Push(a * b);
                }
                if (operation.Equals('/'))
                {
                    float b = valueStack.Pop();
                    float a = valueStack.Pop();
                    valueStack.Push(a / b);
                }
                if (operation.Equals('l'))
                {
                    float b = valueStack.Pop();
                    float a = valueStack.Pop();
                    valueStack.Push(Mathf.Log(a, b));
                }
                if (operation.Equals('e'))
                {
                    float b = valueStack.Pop();
                    float a = valueStack.Pop();
                    valueStack.Push(Mathf.Pow(a, b));
                }
            }
        }
        return valueStack.Pop();
    }
}