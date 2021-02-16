using SpliceMachine.Drda.Helpers;
using SpliceMachine.Drda.Resources;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Xml;

namespace SpliceMachine.Drda
{
    internal sealed class CommAreaGroupDescriptor
    {
        [SuppressMessage("ReSharper", "UnusedVariable")]
        public CommAreaGroupDescriptor(
            DrdaStreamReader reader)
        {
            if (reader.ReadUInt8() == 0xFF)
            {
                return;
            }

            SqlCode = reader.ReadUInt32();
            SqlState = reader.ReadString(5);
            var sqlErrProc = reader.ReadString(8);

            if (reader.ReadUInt8() != 0xFF)
            {
                RowsFetched = reader.ReadUInt64();
                RowsUpdated = reader.ReadUInt32();

                var sqlErrs = reader.ReadBytes(12); // 3 * sizeof(UInt32)
                var sqlWarn = reader.ReadBytes(11); // 11 * sizeof(Byte)

                var rdbName = reader.ReadUInt16();

                SqlMessage = reader.ReadVcmVcs();
            }
            if (!string.IsNullOrEmpty(SqlState))
            {
                var errorsXml = SpliceErrors.ResourceManager.GetString("Errors");
                string errorMsg = String.Empty;
                XmlDocument errorsXmlDoc = new XmlDocument();
                errorsXmlDoc.LoadXml(errorsXml);
                for (int i = 0; i < errorsXmlDoc.LastChild.ChildNodes.Count; i++)
                {
                    if (errorsXmlDoc.LastChild.ChildNodes[i].Attributes["Key"].Value == SqlState)
                    {
                        errorMsg = errorsXmlDoc.LastChild.ChildNodes[i].InnerText;
                        if (!String.IsNullOrEmpty(errorsXmlDoc.LastChild.ChildNodes[i].Attributes["Params"].Value))
                        {
                            var paramsCount = Convert.ToInt16(errorsXmlDoc.LastChild.ChildNodes[i].Attributes["Params"].Value);
                            var replaceTxtArray = SqlMessage.Split(Char.Parse("\u0014"));
                            for (int j = 0; j < paramsCount; j++)
                            {
                                errorMsg = errorMsg.Replace("%" + (j+1).ToString() + "%", replaceTxtArray[j]);
                            }
                        }
                    }
                }
                throw new SpliceException(errorMsg);
            }
            if (reader.ReadUInt8() != 0xFF)
            {
                // WORKWORK
            }
        }

        public UInt32 SqlCode { get; }

        public String SqlState { get; }

        public UInt32 RowsUpdated { get; }

        public UInt64 RowsFetched { get; }

        public String SqlMessage { get; }
    }
}
