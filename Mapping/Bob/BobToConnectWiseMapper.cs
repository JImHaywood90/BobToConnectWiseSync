using Abstractions.Mapping;
using Dto.Bob;
using Dto.ConnectWise;
using Riok.Mapperly.Abstractions;


namespace AzureApiPoc.Mapping.Bob
{
    [Mapper]
    public partial class BobToConnectWiseMapper : IModelMapper<HiBobLeaveDetails, ConnectWiseScheduleEntryPayload>
    {
        public partial ConnectWiseScheduleEntryPayload Map(HiBobLeaveDetails source, int memberId, int scheduleTypeId);

        // Interface implementation using params[] for flexibility
        public ConnectWiseScheduleEntryPayload Map(HiBobLeaveDetails source, params object[] args)
        {
            var memberId = (int)args[0];
            var scheduleTypeId = (int)args[1];
            return Map(source, memberId, scheduleTypeId);
        }

        private void Map(HiBobLeaveDetails source, int memberId, int scheduleTypeId, ConnectWiseScheduleEntryPayload target)
        {
            target.member = new { id = memberId };
            target.scheduleType = new { id = scheduleTypeId };
            target.notes = $"Leave request {source.requestId}";
        }
    }
}
