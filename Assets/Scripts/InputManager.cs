using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class InputManager : MonoBehaviour
{
    const float pi = Mathf.PI;
    const float e = 2.71828F;
    public Text function;
    public Text validateText;
    public Button generate;

    private Dictionary<string, string> parser = new Dictionary<string, string>()
    {
        {"1", "1"}, {"2", "2"}, {"3", "3"}, {"4", "4"}, {"5", "5"}, {"6", "6"}, {"7", "7"}, {"8", "8"}, {"9", "9"}, {"0", "0"},
        {"Dot", "."}, {"Delete", "delete"}, {"Add", "+"}, {"Subtract", "-"}, {"Multiply", "*"}, {"Divide", "/"}, {"X", "x"},
        {"Y", "y"}, {"Z", "z"}, {"Open Parenthesis", "("}, {"Closed Parenthesis", ")"}, {"Pi", "pi"}, {"E", "e"}, {"Log", "log("},
        {"Exp", "exp("}, {"Sin", "sin("}, {"Cos", "cos("}, {"Comma", ","}
    };
    public void addText()
    {
        string defaultFunctionText = "Enter the function f(x,z) to plot the surface y = f(x,z)";
        string defaultValidateText = "Check for validity of your expression before generating surface.";
        if (function.text.Equals(defaultFunctionText))
        {
            function.text = "";
        }

        string input = parser[EventSystem.current.currentSelectedGameObject.name];

        if (input.Equals("delete"))
        {
            function.text = function.text.Substring(0, function.text.Length - 1);
        }
        else
        {
            function.text += input;
        }

        if (function.text.Equals(""))
        {
            function.text = defaultFunctionText;
            validateText.text = defaultValidateText;
        }
    }

    private static string simplify(string f)
    {
        for (int i = 0; i < f.Length; i++)
        {
            if (f[i].Equals('e'))
            {
                if (i < f.Length - 1 && f[i + 1].Equals('x'))
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

    private static bool evaluateString(float u, float v, string f)
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
        valueStack.Pop();
        if (valueStack.Count == 0) return true;
        if (valueStack.Count == 1 && valueStack.Peek() == 0f) return true;
        if (operationStack.Count == 0) return true;
        return false;
    }

    public void validateFunction()
    {
        bool isValid = true;
        string expression = function.text;

        // parentheses check
        int pCount = 0;
        for (int i = 0; i < expression.Length; i++)
        {
            if (expression[i].Equals('('))
            {
                pCount++;
            }
            if (expression[i].Equals(')'))
            {
                pCount--;
            }
            if (pCount < 0)
            {
                isValid = false;
            }
        }
        Debug.Log("isValid " + isValid.ToString());
        isValid = isValid && evaluateString(0f, 0f, expression);
        Debug.Log("evaluateString " + isValid.ToString());
        if (isValid)
        {
            validateText.text = "Vaild!";
        }
        else
        {
            validateText.text = "Invalid!";
        }
    }
}
