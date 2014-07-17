using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Modulo.Collect.OVAL.Definitions.Windows
{
    public partial class service_object : ObjectType
    {
        public override IEnumerable<EntitySimpleBaseType> GetEntityBaseTypes()
        {
            return this.Items.OfType<EntitySimpleBaseType>();
        }
    }
}
