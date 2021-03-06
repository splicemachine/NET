﻿using System;
using System.Text;

namespace SpliceMachine.Drda
{
    public sealed class PiggyBackSchemaDescResponse : DrdaResponseBase
    {
        internal PiggyBackSchemaDescResponse(
            ResponseMessage response)
            : base(response)
        {
            foreach (var parameter in response.Command)
            {
                switch (parameter)
                {
                    case UInt8Parameter para when para.CodePoint == CodePoint.PBSD_ISO:
                        IsolationLevel = para.Value;
                        break;

                    case BytesParameter para when para.CodePoint == CodePoint.PBSD_SCHEMA:
                        Schema = Encoding.UTF8.GetString(para.Value);
                        break;
                }
            }
        }

        public Byte IsolationLevel { get; }

        public String Schema { get; }

        internal override Boolean Accept(
            DrdaStatementVisitor visitor) => visitor.Visit(this);
    }
}
