using System;
using System.Text;

namespace ImageEXIFExtractor
{
    public static class NumberSystemConverter
    {
        public enum Base
        {
            Binary = 2,
            Octal = 8,
            Decimal = 10,
            Hexadecimal = 16
        }

        public static string ConvertDecimalIntegerToBase(int decimalNumber, Base targetBase)
        {
            string convertedValue = string.Empty;
            StringBuilder sbConv = new StringBuilder();
            try
            {
                int temp = decimalNumber;
                if (targetBase != Base.Hexadecimal)
                {
                    while (temp > 0)
                    {
                        int remainder = temp % Convert.ToInt32(targetBase);
                        sbConv.Append(remainder.ToString());
                        temp /= Convert.ToInt32(targetBase);
                    }
                }
                else
                {
                    while (temp > 0)
                    {
                        int remainder = temp % Convert.ToInt32(targetBase);
                        if (remainder == 10)
                        {
                            sbConv.Append("A");
                        }
                        else if (remainder == 11)
                        {
                            sbConv.Append("B");
                        }
                        else if (remainder == 12)
                        {
                            sbConv.Append("C");
                        }
                        else if (remainder == 13)
                        {
                            sbConv.Append("D");
                        }
                        else if (remainder == 14)
                        {
                            sbConv.Append("E");
                        }
                        else if (remainder == 15)
                        {
                            sbConv.Append("F");
                        }
                        else
                        {
                            sbConv.Append(remainder.ToString());
                        }
                        temp /= Convert.ToInt32(targetBase);
                    }
                }
                convertedValue = sbConv.ToString().Trim();
                char[] ary = convertedValue.ToCharArray();
                StringBuilder sbNewString = new StringBuilder();
                for (int i = ary.Length - 1; i >= 0; i--)
                {
                    sbNewString.Append(ary[i].ToString());
                }
                convertedValue = sbNewString.ToString().Trim();
            }
            catch (Exception)
            {
                convertedValue = null;
            }
            return convertedValue;
        }

        public static string GetBinaryInXBitFormat(string binaryValue, uint bitCount)
        {
            string adjustedBinary = string.Empty;
            try
            {
                if (bitCount > binaryValue.Length)
                {
                    StringBuilder sbAdjustedBinary = new StringBuilder();
                    for (uint i = 0; i < bitCount - binaryValue.Length; i++)
                    {
                        sbAdjustedBinary.Append("0");
                    }
                    sbAdjustedBinary.Append(binaryValue);
                    adjustedBinary = sbAdjustedBinary.ToString().Trim();
                }
                else
                {
                    adjustedBinary = binaryValue;
                }
            }
            catch (Exception)
            {
                adjustedBinary = null;
            }
            return adjustedBinary;
        }
    }
}