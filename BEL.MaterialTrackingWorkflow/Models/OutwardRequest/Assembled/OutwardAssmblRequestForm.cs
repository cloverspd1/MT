﻿namespace BEL.MaterialTrackingWorkflow.Models.OutwardRequest
{
   using BEL.CommonDataContract;
    using BEL.MaterialTrackingWorkflow.Models.Common;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    /// <summary>
    /// Outward Assmbl Request Form
    /// </summary>
    [DataContract, Serializable]
    public class OutwardAssmblRequestForm : IForm
    {
                 /// <summary>
        /// Initializes a new instance of the <see cref="DRForm"/> class.
        /// </summary>
        public OutwardAssmblRequestForm()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DRForm"/> class.
        /// </summary>
        /// <param name="setSections">if set to <c>true</c> [set sections].</param>
        public OutwardAssmblRequestForm(bool setSections)
        {
            if (setSections)
            {
                this.ApprovalMatrixListName = MaterialTrackingListNames.OUTWARDREQUESTSAPPAPPROVALMATRIX;
                this.SectionsList = new List<ISection>();
                this.SectionsList.Add(new TesterSection(true));  ////LEVEL 0
                this.SectionsList.Add(new HODSection(true));        ////LEVEL 1
                this.SectionsList.Add(new RecipientSection(true)); 
                this.SectionsList.Add(new ApplicationStatusSection(true) { SectionName = SectionNameConstant.APPLICATIONSTATUS });
                this.SectionsList.Add(new ActivityLogSection(MaterialTrackingListNames.OUTWARDREQUESTSACTIVITYLOG));
                this.Buttons = new List<Button>();
                this.MainListName = MaterialTrackingListNames.OUTWARDREQUESTSLIST;
            }
        }

        /// <summary>
        /// Gets the name of the form.
        /// </summary>
        /// <value>
        /// The name of the form.
        /// </value>
        [DataMember]
        public string FormName
        {
            get { return FormNames.OUTWARDASSMBLREQUESTFORM; }
        }

        /// <summary>
        /// Gets or sets the form status.
        /// </summary>
        /// <value>
        /// The form status.
        /// </value>
        [DataMember]
        public string FormStatus { get; set; }

        /// <summary>
        /// Gets or sets the form approval level.
        /// </summary>
        /// <value>
        /// The form approval level.
        /// </value>
        [DataMember]
        public int FormApprovalLevel { get; set; }

        /// <summary>
        /// Gets or sets the total approval required.
        /// </summary>
        /// <value>
        /// The total approval required.
        /// </value>
        [DataMember]
        public int TotalApprovalRequired { get; set; }

        /// <summary>
        /// Gets or sets the sections list.
        /// </summary>
        /// <value>
        /// The sections list.
        /// </value>
        [DataMember]
        public List<ISection> SectionsList { get; set; }

        /// <summary>
        /// Gets or sets the buttons.
        /// </summary>
        /// <value>
        /// The buttons.
        /// </value>
        [DataMember]
        public List<Button> Buttons { get; set; }

        /// <summary>
        /// Gets or sets the name of the approval matrix list.
        /// </summary>
        /// <value>
        /// The name of the approval matrix list.
        /// </value>
        [DataMember]
        public string ApprovalMatrixListName { get; set; }

        /// <summary>
        /// Gets or sets the name of the main list.
        /// </summary>
        /// <value>
        /// The name of the main list.
        /// </value>
        [DataMember]
        public string MainListName { get; set; }
    }
}