using System;
using System.Collections.Generic;
using System.Text;

namespace ACRealms.Roslyn.RealmProps
{
    internal record PropContext
    {
        public required string Name { get; init; }
        public required string Entity { get; init; }
        public required string? Description { get; init; }
        public required bool Required { get; init; }
    }
}
