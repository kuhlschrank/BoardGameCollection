using System;
using System.Collections.Generic;
using System.Text;

namespace BoardGameCollection.Core.Models
{
    public interface IHasBoardGame
    {
        BoardGame BoardGame { get; }
    }
}
