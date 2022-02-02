using System;
using System.ComponentModel;

namespace Egate_Ecommerce
{
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum KoreaShipStatus
    {
        [Description("Not Received")]
        NotReceived,
        [Description("Received")]
        Received,
        [Description("Received from Other Location")]
        ReceivedFromOtherLocation
    }
}
