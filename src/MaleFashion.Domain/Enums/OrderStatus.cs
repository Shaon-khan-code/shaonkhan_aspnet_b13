using System;
using MaleFashion.Domain.Common;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Domain.Enums
{
    public enum OrderStatus
    {
        Pending,
        Processing,
        Shipped,
        Delivered,
        Cancelled
    }
}
