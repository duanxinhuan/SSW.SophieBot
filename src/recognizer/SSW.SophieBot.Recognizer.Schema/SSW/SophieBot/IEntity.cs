﻿using System;
using System.Collections.Generic;

namespace SSW.SophieBot
{
    public interface IEntity : IModel
    {
        ICollection<Type> Children { get; }
    }
}
