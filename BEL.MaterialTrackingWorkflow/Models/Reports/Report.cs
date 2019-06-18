using BEL.CommonDataContract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace BEL.MaterialTrackingWorkflow.Models.Reports
{
    [DataContract, Serializable]
    public class Report
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Report"/> class.
        /// </summary>
        public Report()
        {
            ReportList = new List<ReportDetails>();
            BUList = new List<NameValueData>();
            MaterialLocationList = new List<NameValueData>();
        }

        /// <summary>
        /// Gets or sets the project identifier.
        /// </summary>
        /// <value>
        /// The project identifier.
        /// </value>
        [DataMember]
        public string ProjectName { get; set; }

        /// <summary>
        /// Gets or sets the date values.
        /// </summary>
        /// <value>
        /// The date values.
        /// </value>
        [DataMember, DataType(DataType.Date)]
        public DateTime? DateValues { get; set; }

        /// <summary>
        /// Gets or sets the serial number.
        /// </summary>
        /// <value>
        /// The serial number.
        /// </value>
        [DataMember]
        public string SerialNumber { get; set; }

        /// <summary>
        /// Gets or sets the material location.
        /// </summary>
        /// <value>
        /// The material location.
        /// </value>
        [DataMember]
        public string MaterialLocation { get; set; }

        /// <summary>
        /// Gets or sets the bu identifier.
        /// </summary>
        /// <value>
        /// The bu identifier.
        /// </value>
        [DataMember]
        public string BUName { get; set; }

        /// <summary>
        /// Gets or sets the report list.
        /// </summary>
        /// <value>
        /// The report list.
        /// </value>
        [DataMember]
        public List<ReportDetails> ReportList { get; set; }

        /// <summary>
        /// Gets or sets the bu list.
        /// </summary>
        /// <value>
        /// The bu list.
        /// </value>
        [DataMember]
        public List<NameValueData> BUList { get; set; }

        /// <summary>
        /// Gets or sets the material location list.
        /// </summary>
        /// <value>
        /// The material location list.
        /// </value>
        [DataMember]
        public List<NameValueData> MaterialLocationList { get; set; }
    }
}