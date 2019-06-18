
namespace BEL.MaterialTrackingWorkflow.Models.Common
{
    /// <summary>
    /// Form Names
    /// </summary>
    public static class FormNames
    {
        /// <summary>
        /// The DCNform
        /// </summary>
        public const string INWARDREQUESTFORM = "Inward Request Form"; 

        /// <summary>
        /// The DCRform
        /// </summary>
        public const string OUTWARDASSMBLREQUESTFORM = "Outward Assembled Request Form";

        /// <summary>
        /// The DCR Admin form
        /// </summary>
        public const string OUTWARDSINGLEREQUESTFORM = "Outward Single Request Form";
    }

    /// <summary>
    /// List Names
    /// </summary>
    public static class MaterialTrackingListNames
    {
        /// <summary>
        /// The INWARD list name
        /// </summary>
        public const string INWARDLIST = "INWARD";

        /// <summary>
        /// The INWARD activity log
        /// </summary>
        public const string INWARDACTIVITYLOG = "InwardActivityLog";

        /// <summary>
        /// The INWARD appapproval matrix
        /// </summary>
        public const string INWARDAPPAPPROVALMATRIX = "InwardApprovalMatrix";

  
        /// <summary>
        /// The OutwardRequests list
        /// </summary>
        public const string OUTWARDREQUESTSLIST = "OutwardRequests";

        /// <summary>
        /// The OutwardRequests activity log
        /// </summary>
        public const string OUTWARDREQUESTSACTIVITYLOG = "OutwardRequestsActivityLog";

        /// <summary>
        /// The OutwardRequests Approval Matrix
        /// </summary>
        public const string OUTWARDREQUESTSAPPAPPROVALMATRIX = "OutwardRequestsApprovalMatrix";

        /// <summary>
        /// The OutwardRequests list
        /// </summary>
        public const string OUTWARDSINGLEREQUESTSLIST = "OutwardSingleRequests";

        /// <summary>
        /// The OutwardRequests activity log
        /// </summary>
        public const string OUTWARDSINGLEREQUESTSACTIVITYLOG = "OutwardSingleRequestsActivityLog";

        /// <summary>
        /// The OutwardRequests Approval Matrix
        /// </summary>
        public const string OUTWARDSINGLEREQUESTSAPPAPPROVALMATRIX = "OutwardSingleRequestsApprovalMatrix";

        /// <summary>
        /// the TYPE OF Material Master
        /// </summary>
        public const string TYPEOFMATERIALMASTER = "TypeofMaterialMaster";

        /// <summary>
        /// the TYPE OF Courier Type Master
        /// </summary>
        public const string COURIERTYPEMASTER = "CourierTypeMaster";

        /// <summary>
        /// the TYPE OF Business Unit Master
        /// </summary>
        public const string BUSINESSUNITMASTER = "BusinessUnitMaster";

        /// <summary>
        /// the TYPE OF Business Unit Master
        /// </summary>
        public const string PROJECTNAMEMASTER = "ProjectNameMaster";

        /// <summary>
        /// the TYPE OF Business Unit Master
        /// </summary>
        public const string PRODUCTCATEGORYMASTER = "ProductCategoryMaster";

        /// <summary>
        /// The TYPE OF Material Category
        /// </summary>
        public const string MATERIALCATEGORY = "MaterialCategory";

        /// <summary>
        /// The TYPE OF Material Category
        /// </summary>
        public const string MATERIALLOCATION = "MaterialLocation";

        /// <summary>
        /// the TYPE OF Business Unit Master
        /// </summary>
        public const string INWARDREQUESTNOCOUNT = "InwardRequestNoCount";

        /// <summary>
        /// the TYPE OF Business Unit Master
        /// </summary>
        public const string OUTWARDREQUESTNOCOUNT = "OutwardRequestNoCount";

        /// <summary>
        /// the TYPE OF Business Unit Master
        /// </summary>
        public const string RecipientAttachmentRecipientAttachmentRecipientAttachment = "OutwardRequestNoCount";

        /// <summary>
        /// the TYPE OF Business Unit Master
        /// </summary>
        public const string SERIALNOCOUNTCOUNT = "SerialNoCount";
        

        /// <summary>
        /// the TYPE OF Approver Master
        /// </summary>
        public const string APPROVERMASTER = "ApproverMaster";

        /// <summary>
        /// the TYPE OF Recipient 1 Master
        /// </summary>
        public const string RECIPIENT1MASTER = "Recipient1Master";

        /// <summary>
        /// the TYPE OF Recipient 2 Master
        /// </summary>
        public const string RECIPIENT2MASTER = "Recipient2Master";

        /// <summary>
        /// the TYPE OF Tester Master
        /// </summary>
        public const string TESTERMASTER = "TesterMaster";

        /// <summary>
        /// the TYPE OF Approver Master
        /// </summary>
        public const string EMPLOYEEMASTER = "EmployeeMaster";

        /// <summary>
        /// The declarationtextmaster
        /// </summary>
        public const string DECLARATIONTEXTMASTER = "DeclarationTextMaster";

        /// <summary>
        /// The rolemaster
        /// </summary>
        public const string ROLEMASTER = "RoleMaster";

        /// <summary>
        /// The rolemenumapping
        /// </summary>
        public const string ROLESCREENMAPPING = "RoleScreenMapping";

        /// <summary>
        /// The screenmaster
        /// </summary>
        public const string SCREENMASTER = "ScreenMaster";

    }

    public static class Masters
    {
        /// <summary>
        /// the MeetingRelatedto
        /// </summary>
        public const string TYPEOFMATERIALMASTER = "TypeofMaterialMaster";

        /// <summary>
        /// the Product Category Master
        /// </summary>
        public const string COURIERTYPEMASTER = "CourierTypeMaster";

        /// <summary>
        /// The declarationtextmaster
        /// </summary>
        public const string DECLARATIONTEXTMASTER = "DeclarationTextMaster";

        /// <summary>
        /// the Design business Master
        /// </summary>

        public const string BUSINESSUNITMASTER = "BusinessUnitMaster";

        /// <summary>
        /// the TYPE OF Business Unit Master
        /// </summary>
        public const string PROJECTNAMEMASTER = "ProjectNameMaster";

        /// <summary>
        /// the TYPE OF Business Unit Master
        /// </summary>
        public const string PRODUCTCATEGORYMASTER = "ProductCategoryMaster";

        /// <summary>
        /// the TYPE OF Material Category
        /// </summary>
        public const string MATERIALCATEGORY = "MaterialCategory";

        /// <summary>
        /// the TYPE OF Material Location
        /// </summary>
        public const string MATERIALLOCATION = "MaterialLocation";
    }

    /// <summary>
    /// DCR Section Names
    /// </summary>
    public static class INWARDSectionName
    {
        /// <summary>
        /// The dcr details section
        /// </summary>
        public const string RECIPIENT1SECTION = "Recipient 1 Section";

        /// <summary>
        /// The DCR Incharge Navigator Section
        /// </summary>
        public const string RECIPIENT2SECTION = "Recipient 2 Section";

        /// <summary>
        /// The activitylog
        /// </summary>
        public const string ACTIVITYLOG = "Activity Log";
        /// <summary>
        /// The activitylog
        /// </summary>
        public const string APPLICATIONSTATUSSECTION = "Application Status Section";     
    }

    /// <summary>
    /// DCN Section Names Section
    /// </summary>
    public static class OutwardSectionName
    {
        /// <summary>
        /// The dcr details section
        /// </summary>
        public const string TESTERSECTION = "Tester Section";

        /// <summary>
        /// The DCR Incharge Navigator Section
        /// </summary>
        public const string HODSECTION = "HOD Section";

        /// <summary>
        /// The DCR Incharge Navigator Section
        /// </summary>
        public const string RECIPIENTSECTION = "Recipient 1 Section";

        /// <summary>
        /// The dcr details section
        /// </summary>
        public const string TESTERSINGLESECTION = "Tester Section";

        /// <summary>
        /// The DCR Incharge Navigator Section
        /// </summary>
        public const string HODSINGLESECTION = "HOD Section";

        /// <summary>
        /// The DCR Incharge Navigator Section
        /// </summary>
        public const string RECIPIENTSINGLESECTION = "Recipient 1 Section";

        /// <summary>
        /// The activitylog
        /// </summary>
        public const string ACTIVITYLOG = "Activity Log";
        /// <summary>
        /// The activitylog
        /// </summary>
        public const string APPLICATIONSTATUSSECTION = "Application Status Section";  
    }

    /// <summary>
    /// DCR Roles
    /// </summary>
    public static class INWARDRoles
    {
        /// <summary>
        /// The creator
        /// </summary>
        public const string RECIPIENT1 = "Recipient 1";

        /// <summary>
        /// The viewer
        /// </summary>
        public const string VIEWER = "Viewer";

        /// <summary>
        /// The editor
        /// </summary>
        public const string EDITOR = "Editor";

        /// <summary>
        /// DCR Incharge Navigator
        /// </summary>
        public const string RECIPIENT2 = "Recipient 2";

        /// <summary>
        /// DCR Incharge Navigator
        /// </summary>
        public const string TESTER = "Tester";
    }

    /// <summary>
    /// DCN Roles
    /// </summary>
    public static class OUTWARDRoles
    {
        /// <summary>
        /// The creator
        /// </summary>
        public const string CREATOR = "Creator";

        /// <summary>
        /// The creator
        /// </summary>
        public const string TESTER = "Tester";

        /// <summary>
        /// The viewer
        /// </summary>
        public const string VIEWER = "Viewer";

        /// <summary>
        /// SCM
        /// </summary>
        public const string HOD = "HOD";

        /// <summary>
        /// The editor
        /// </summary>
        public const string EDITOR = "Editor";

        /// Design Document Engineer
        /// </summary>
        public const string RECIPIENT1 = "Recipient 1";

        /// <summary>
        /// Design Engineer
        /// </summary>
        public const string RECIPIENT2 = "Recipient 2";
     }

}