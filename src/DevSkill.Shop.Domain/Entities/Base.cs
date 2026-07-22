using System;
using System.Collections.Generic;
using System.Text;

namespace DevSkill.Shop.Domain.Entities
{
    public abstract class Entity<TKey>
    {
        public TKey Id { get; set; } = default!;
    }
}

