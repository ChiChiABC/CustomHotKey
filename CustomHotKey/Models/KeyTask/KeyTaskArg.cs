﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CustomHotKey.Models.KeyTask
{
    public class KeyTaskArg
    {
        public string? ArgValue { get; set; } = "";

        public static implicit operator string(KeyTaskArg arg)
        {
            return arg.ArgValue;
        }
        //  User-defined conversion from double to Digit
        public static implicit operator KeyTaskArg(string s)
        {
            return new KeyTaskArg() { ArgValue = s };
        }

        public override string? ToString()
        {
            return ArgValue;
        }
    }
}
