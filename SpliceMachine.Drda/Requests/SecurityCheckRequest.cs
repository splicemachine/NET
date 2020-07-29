using System;
using System.Text;

namespace SpliceMachine.Drda
{
    public sealed class SecurityCheckRequest
        : DrdaRequestBase<SecurityCheckResponse>, IDrdaRequest
    {
        private readonly String _userName;

        private readonly String _password;

        public SecurityCheckRequest(
            UInt16 requestCorrelationId,
            String userName,
            String password)
            : base(
                requestCorrelationId)
        {
            _userName = userName;
            _password = password;
        }

        CompositeCommand IDrdaRequest.GetCommand() =>
            new CompositeCommand(
                CodePoint.SECCHK,
                new UInt16Parameter(CodePoint.SECMEC, 0x0003), // USRIDPWD

                // TODO: olegra - check if it really needed
                //command.WriteParameter(0x2110, WellKnownStrings.DatabaseName); // RDBNAM

                Encoding.UTF8.GetParameter(CodePoint.PASSWORD, _password), // 
                Encoding.UTF8.GetParameter(CodePoint.USRID, _userName) // 
            );
    }
}
