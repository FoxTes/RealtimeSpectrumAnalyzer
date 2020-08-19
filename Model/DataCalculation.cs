using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Real_time_Spectrum_Analyzer.Model
{
    public static class DataCalculation
    {
        public const UInt16 maskByte = 0b0000_1111_1111_1111;
        public static Double Convert12Bit(Byte firstByte, Byte secondByte)
        {
            var result = (firstByte | secondByte << 0x07) & maskByte;

            // Если результат превышает 2047 (10 бит), то необходимо произвести вычитание,
            // т.к. значение является отрицательным.
            if (result < 0x07FF)
            {
                return result / 10f;
            }
            else
            {
                return (result - 0x0FFF) / 10f;
            }
        }

        public static Double ConvertAbs(Byte firstByte, Byte secondByte)
        {
            var result = (firstByte | secondByte << 0x07) & maskByte;

            // Вычесление по модулю.
            if (result > 0x07FF)
            {
                return (0x0FFF - result) / 10f;
            }
            return result / 10f;
        }

        public static void ShiftLeft(double[] Arr)
        {
            double temp = Arr[0];
            for (int i = 0; i < Arr.Length - 1; i++)
            {
                Arr[i] = Arr[i + 1];
            }
            Arr[Arr.Length - 1] = temp;
        }
    }
}
