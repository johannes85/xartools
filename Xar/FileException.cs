﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xar
{
    public class FileException : Exception
    {
        public FileException(string message)
            : base(message)
        {

        }
    }
}
