﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpeechCustomUi
{
    class Globals
    {
        private static string _text { get; set; }
        public static string text 
        {
            get
            {
                return _text ;
            } 
            set
            {
                _text = value;
            }
        }

        private static string _weather { get; set; }
        public static string weather
        {
            get
            {
                return _weather;
            }
            set
            {
                _weather = value;
            }
        }
    }
}
