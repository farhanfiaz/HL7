using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using HL7.Dotnetcore;
using HL7_API.Model;
using System.Reflection;
using System.Text;

namespace HL7_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class HL7Controller : ControllerBase
    {
        public HL7Controller()
        {

        }
        [HttpPost(Name = "SendToHL7")]
        public async Task<IActionResult> SendToHL7(List<HL7DataModel> modelList)
        {
            if (modelList.Count == 0)
            {
                return BadRequest("HL7 Data Model List Required..!");
            }
            foreach (var item in modelList)
            {
                var HL7FormatResponce = GetHL7Format(item);
                HL7FormatResponce = HL7FormatResponce.Replace("*", "\\");
                // Write the HL7 data to a text file
                var filePath = "/" + item.CaseNumber + ".hl7";
                System.IO.File.WriteAllText(filePath, HL7FormatResponce);
                //// Upload HL7 file to remote ftp server
                // string response = uploadToFTPServer();
                // if (File.Exists(filePath))
                // {
                //     // Delete the file
                //     File.Delete(filePath);
                // }
                item.Success = true;
            }
            return Ok(modelList);
        }
        private static string GetHL7Format(HL7DataModel model)
        {
            Message oHl7Message = new Message();
            var currentDateTime = DateTime.UtcNow.ToString("yyyyMMddhhmmss");
            #region MSH Segment
            // Add MSH Segment
            Segment mshSegment = new Segment("MSH", new HL7Encoding());
            mshSegment = AddSpaceByUsingLoopNumber(ref mshSegment, 1);
            mshSegment.AddNewField(ComponentDelimiterRepeat(ref mshSegment, 1)+"~*&");
            mshSegment.AddNewField("TeleCare");
            mshSegment = AddSpaceByUsingLoopNumber(ref mshSegment, 1);
            mshSegment.AddNewField("VNT_Telecare");
            mshSegment = AddSpaceByUsingLoopNumber(ref mshSegment, 1);
            //Current Date Time
            mshSegment.AddNewField(currentDateTime);
            mshSegment = AddSpaceByUsingLoopNumber(ref mshSegment, 1);
            mshSegment.AddNewField("DFT^P03");
            //Case Number
            mshSegment.AddNewField(model.CaseNumber);
            mshSegment.AddNewField("P");
            mshSegment.AddNewField("2.3");
            mshSegment = AddSpaceByUsingLoopNumber(ref mshSegment, 3);
            mshSegment.AddNewField("NE");
            oHl7Message.AddNewSegment(mshSegment);
            #endregion
            #region EVN
            // Add EVN Segment
            Segment eVNSegment = new Segment("EVN", new HL7Encoding());
            eVNSegment.AddNewField("P03");
            eVNSegment.AddNewField(currentDateTime);
            //eVNSegment.AddEmptyField();
            eVNSegment = AddSpaceByUsingLoopNumber(ref eVNSegment,1);
            eVNSegment.AddNewField("02");
            oHl7Message.AddNewSegment(eVNSegment);
            #endregion
            #region PID
            // Add PID Segment
            Segment pidSegment = new Segment("PID", new HL7Encoding());
            pidSegment.AddNewField("1");
            //Identification
            pidSegment.AddNewField(model.Identification);
            pidSegment.AddNewField(model.Identification);
            pidSegment.AddNewField(model.Identification);
            //Last Name & First Name
            pidSegment.AddNewField(model.LastName+ ComponentDelimiterRepeat(ref pidSegment, 1) + model.FirstName);
            pidSegment = AddSpaceByUsingLoopNumber(ref pidSegment, 1);
            pidSegment.AddNewField("DOB");
            pidSegment.AddNewField("Gender");
            pidSegment = AddSpaceByUsingLoopNumber(ref pidSegment, 9);
            pidSegment.AddNewField(model.FIN??string.Empty);
            pidSegment = AddSpaceByUsingLoopNumber(ref pidSegment, 11);
            pidSegment.AddNewField("N");
            oHl7Message.AddNewSegment(pidSegment);
            #endregion
            #region PV1
            // Add PV1 Segment
            Segment pv1Segment = new Segment("PV1", new HL7Encoding());
            pv1Segment.AddNewField("1");
            pv1Segment.AddNewField("0");
            pv1Segment.AddNewField(model.TSAccountId + ComponentDelimiterRepeat(ref pv1Segment, 3) + model.TSAccountId + ComponentDelimiterRepeat(ref pv1Segment, 5) + model.FacilityName);
            pv1Segment = AddSpaceByUsingLoopNumber(ref pv1Segment, 3);
            pv1Segment.AddNewField(model.NPINumber + ComponentDelimiterRepeat(ref pv1Segment, 1) + model.PhysicianLastName + ComponentDelimiterRepeat(ref pv1Segment, 1) + model.PhysicianFirstName);
            pv1Segment = AddSpaceByUsingLoopNumber(ref pv1Segment, 11);
            pv1Segment.AddNewField(model.FIN ?? string.Empty);
            pv1Segment = AddSpaceByUsingLoopNumber(ref pv1Segment, 19);
            pv1Segment.AddNewField(model.FacilityName);
            pv1Segment = AddSpaceByUsingLoopNumber(ref pv1Segment, 4);
            pv1Segment.AddNewField(model.DateOfConsult.ToString("yyyyMMddhhmmss"));
            pv1Segment = AddSpaceByUsingLoopNumber(ref pv1Segment, 5);
            pv1Segment.AddNewField(model.CaseNumber);
            oHl7Message.AddNewSegment(pv1Segment);
            #endregion
            #region FT1
            // Add FT1 Segment
            Segment ft1Segment = new Segment("FT1", new HL7Encoding());
            ft1Segment.AddNewField("1");
            ft1Segment = AddSpaceByUsingLoopNumber(ref ft1Segment, 2);
            ft1Segment.AddNewField(model.DateOfConsult.ToString("yyyyMMddhhmmss"));
            ft1Segment.AddNewField(currentDateTime);
            ft1Segment.AddNewField("CG");
            ft1Segment.AddNewField(model.CPTCode + ComponentDelimiterRepeat(ref ft1Segment,2) + "CPT");
            ft1Segment = AddSpaceByUsingLoopNumber(ref ft1Segment, 2);
            ft1Segment.AddNewField("1");
            ft1Segment = AddSpaceByUsingLoopNumber(ref ft1Segment, 5);
            ft1Segment.AddNewField(ComponentDelimiterRepeat(ref ft1Segment, 3) + model.TSAccountId+ ComponentDelimiterRepeat(ref ft1Segment, 5) + "MH MAIN OR");
            ft1Segment = AddSpaceByUsingLoopNumber(ref ft1Segment, 2);
            ft1Segment.AddNewField("003.39" + ComponentDelimiterRepeat(ref ft1Segment, 2) + "I10~H23.23" + ComponentDelimiterRepeat(ref ft1Segment, 2) + "I10");
            ft1Segment.AddNewField(model.NPINumber+ ComponentDelimiterRepeat(ref ft1Segment, 1) + model.PhysicianLastName+ ComponentDelimiterRepeat(ref ft1Segment, 1) + model.PhysicianFirstName);
            ft1Segment = AddSpaceByUsingLoopNumber(ref ft1Segment, 4);
            ft1Segment.AddNewField(model.CPTCode);
            ft1Segment = AddSpaceByUsingLoopNumber(ref ft1Segment, 1);
            oHl7Message.AddNewSegment(ft1Segment);
            #endregion
            string oRetMessage = oHl7Message.SerializeMessage(false);
            return oRetMessage;
        }
        private static string ComponentDelimiterRepeat(ref Segment segment,int loopCount)
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < loopCount; i++)
            {
                stringBuilder.Append(segment.Encoding.ComponentDelimiter);
            }
            return stringBuilder.ToString();
        }
        private static Segment AddSpaceByUsingLoopNumber(ref Segment segment,int LoopCount)
        {
            for (int i = 0; i < LoopCount; i++)
            {
                segment.AddEmptyField();
            }
            return segment;
        }
        public static string UploadToFTPServer()
        {
            string ftpServerUrl = "ftp://waws-prod-blu-205.ftp.azurewebsites.windows.net/site/wwwroot";
            string ftpUserName = "telespecialistsuat__qms\\$telespecialistsuat__qms";
            string ftpPassword = "N9pzy2zY5KGCs1G8e9chDtzYhHmv860B4ogaYEfEZtjAJwclxoxFiwBC3LkE";
            string filePath = "/billing.hl7";
            string remoteFileName = "billing.hl7";
            string uploadingResponse = string.Empty;



            try
            {
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create($"{ftpServerUrl}/{remoteFileName}");
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(ftpUserName, ftpPassword);


                using (Stream fileStream = System.IO.File.OpenRead(filePath))
                {
                    using (Stream ftpStream = request.GetRequestStream())
                    {
                        fileStream.CopyTo(ftpStream);
                    }
                }



                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                uploadingResponse = ($"File upload status: {response.StatusDescription}");
                response.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error uploading file: {ex.Message}");
            }
            return uploadingResponse;
        }
        private static string HL7Formating()
        {
            return $@"MSH|^~\&|Telecare||VNT_Telecare||${DateTime.Now.ToString()}||DFT^P03|$Telecare Case Number|P|2.3||||NE
EVN|P03|$Currentdatetime||02
PID|1|$Identification#|$Identification#|$Identification#|LastName^FirstName||DOB|Gender||||||||||$FIN||||||||||||N
PV1|1|O|$tsaccountid^^^$tsaccountid^^^^^$FacilityName||||$NPINumber^$LastNameofCasePhysician^FirstNameofCasePhysician||||||||||||$FIN||||||||||||||||||||$FacilityName|||||$DateofConsult||||||$Telecare Case Number

FT1|1|||$DateofConsult|20230326081503|CG|$CPTCode^^CPT|||1||||||^^^tsaccountid^^^^^MH MAIN OR|||O03.39^^I10~H23.23^^I10|$NPINumber^$LastNameofCasePhysician^FirstNameofCasePhysician|||||$CPTCode|";
        }
    }
}
