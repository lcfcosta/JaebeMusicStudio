﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace JaebeMusicStudio.Sound
{
    class Flanger : Effect
    {
        List<float> frequency = new List<float>(), amplitude = new List<float>();
        long counter = 0;
        List<float[,]> history = new List<float[,]>();

        public Flanger(XmlElement xml)
        {
            foreach (XmlElement x in xml.ChildNodes)
            {
                frequency.Add(float.Parse(x.Attributes["frequency"].Value, System.Globalization.CultureInfo.InvariantCulture));
                amplitude.Add(float.Parse(x.Attributes["amplitude"].Value, System.Globalization.CultureInfo.InvariantCulture));
            }
        }

        public float[,] doFilter(float[,] input)
        {
            var len1 = input.GetLength(1);
            var ret = new float[input.GetLength(0), input.GetLength(1)];
            for (int n = 0; n < frequency.Count; n++)
            {
                var amplitude_sample = amplitude[n] * Project.current.sampleRate;
                var ileNaCykl = 1 / frequency[n] * Project.current.sampleRate / Math.PI / 2;
                for (int i = 0; i < len1; i++)
                {

                    var z = amplitude_sample * (-1 - Math.Sin((i + counter) / ileNaCykl));
                    var x = (int)z;
                    var proporcje = z - x;
                    if (x >= 0)
                    {
                        ret[0, i] += ((float)(input[0, x] * (1 - proporcje) + input[0, x + 1] * proporcje) / 2);
                        ret[1, i] += ((float)(input[1, x] * (1 - proporcje) + input[1, x + 1] * proporcje) / 2);
                    }
                    else
                    {
                        float input_x00, input_x01, input_x10, input_x11;
                        var historyPosition = history.Count;
                        do
                        {
                            historyPosition--;
                            x += history[historyPosition].GetLength(1);
                        } while (x < 0);
                        input_x00 = history[historyPosition][0, x];
                        input_x10 = history[historyPosition][1, x];
                        if (x == history[historyPosition].GetLength(1))
                        {
                            input_x01 = history[historyPosition + 1][0, 0];
                            input_x11 = history[historyPosition + 1][1, 0];
                        }
                        else
                        {
                            input_x01 = history[historyPosition][0, x + 1];
                            input_x11 = history[historyPosition][1, x + 1];
                        }
                        ret[0, i] += ((float)(input_x00 * (1 - proporcje) + input_x01 * proporcje) / 2);
                        ret[1, i] += ((float)(input_x10 * (1 - proporcje) + input_x11 * proporcje) / 2);
                    }
                }
            }
            counter += input.GetLength(1);
            return ret;
        }
    }
}