using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Regular.Models
{
    public enum RuleTypes
    {
        None,
        AnyLetter,
        SpecificLetter,
        AnyNumber,
        SpecificNumber,
        AnyCharacter,
        SpecificCharacter,
        AnyFromSet,
        Anything,
        Dot,
        Hyphen,
        Underscore
    }
}
