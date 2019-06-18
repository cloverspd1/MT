﻿using System;
using System.Runtime.Serialization;

namespace BEL.MaterialTrackingWorkflow.Models
{
    [Serializable]
    public class OutwardNoCount
    {
        /// <summary>
        /// Item ID
        /// </summary>
        [DataMember]
        public int ID { get; set; }

        /// <summary>
        /// Year Value
        /// </summary>
        [DataMember]
        public int Year { get; set; }

        /// <summary>
        /// Current Value
        /// </summary>
        [DataMember]
        public int CurrentValue { get; set; }
    }
}