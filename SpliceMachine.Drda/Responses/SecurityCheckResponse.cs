using System;

namespace SpliceMachine.Drda
{
    public sealed class SecurityCheckResponse : DrdaResponseBase
    {
        internal SecurityCheckResponse(
            ResponseMessage response)
            : base(response)
        {
            foreach (var parameter in response.Command)
            {
                switch (parameter)
                {
                    case BytesParameter para when para.CodePoint == CodePoint.RDBNAM:
                        RelationalDatabaseName = EncodingEbcdic.GetString(para.Value);
                        break;
                }
            }
        }

        public String RelationalDatabaseName { get; }
    }
}
