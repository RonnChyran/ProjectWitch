using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectWitch
{
    public class ProjectWitchException : Exception
    {
        public ProjectWitchException()
        {

        }

        public ProjectWitchException(string message)
            : base(message)
        {

        }

        public ProjectWitchException(string message, Exception inner)
            : base(message, inner)
        {

        }
    }
}