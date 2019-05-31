using System;
using System.Collections.Generic;
using System.Windows.Media;

namespace Grafik.Controllers
{
    public struct Function
    {
        public int ID { get; }
        public List<Tuple<uint, double>> Numbers { get; }
        public SolidColorBrush Brush { get; }
        public string ReadableForm { get; }

        public Function(List<Tuple<uint, double>> Numbers)
        {
            this.Numbers = Numbers;

            //get random color of Brush
            var rand = new Random();
            var r = (byte)rand.Next(1, 255);
            var g = (byte)rand.Next(1, 255);
            var b = (byte)rand.Next(1, 255);
            Brush = new SolidColorBrush(Color.FromRgb(r, b, g));

            //set next free ID
            Controller.CurrentID++;
            ID = Controller.CurrentID;

            //silmpifying list of numbers
            Numbers.Sort();
            Numbers.Reverse();
            for (int i = 0; i < Numbers.Count - 1; i++)
            {
                if (Numbers[i].Item1 == Numbers[i + 1].Item1)
                {
                    Numbers[i] = new Tuple<uint, double>(Numbers[i].Item1, Numbers[i].Item2 + Numbers[i + 1].Item2);
                    Numbers.RemoveAt(i + 1);
                    i--;
                }
            }

            //creating readable form for items of listBox
            ReadableForm = "f(x)=";
            for (int i = 0; i < Numbers.Count; i++)
            {
                if (i != 0)
                    ReadableForm += '+';
                ReadableForm += Numbers[i].Item2.ToString() + "x^" + Numbers[i].Item1.ToString();
            }
        }

        public double GetValueInX(double x)
        {
            double value = 0;

            //calculate value of function in point x
            foreach (var i in Numbers)
                value += i.Item2 * Math.Pow(x, i.Item1);

            return value;
        }
    }

    static class Controller
    {
        //variables commonly used in application
        public static int CurrentID = 0;
        public static List<Function> Functions = new List<Function>();
    }
}
