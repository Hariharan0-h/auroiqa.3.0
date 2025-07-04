using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroiQa.Domain.Models
{
    [Table("patient_cataract_qa")]
    public class PatientCataractQa
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long? UID { get; set; }

        [StringLength(255)]
        public string? PATIENT_NAME { get; set; }

        public long? Peid { get; set; }

        [StringLength(50)]
        public string? Mr_no { get; set; }

        public DateTime? SurgeryDate { get; set; }

        [StringLength(255)]
        public string? Lensname { get; set; }

        [StringLength(255)]
        public string? Modelname { get; set; }

        [StringLength(50)]
        public string? IolPower { get; set; }

        [StringLength(50)]
        public string? Eye { get; set; }

        [StringLength(100)]
        public string? Patient_Type { get; set; }

        [StringLength(100)]
        public string? Alias { get; set; }

        [StringLength(100)]
        public string? Firstname { get; set; }

        [StringLength(255)]
        public string? SurgeryType { get; set; }

        public long? Siteid { get; set; }

        public DateTime? First_advised_date { get; set; }

        [StringLength(255)]
        public string? Anesthesia { get; set; }

        [StringLength(500)]
        public string? Anesthesia_complication { get; set; }

        public string? ListofIntraopIntraopcomplication { get; set; }

        [StringLength(500)]
        public string? FromdropdownIntraopIntraopcomplication { get; set; }

        [StringLength(100)]
        public string? With_Without_VD { get; set; }

        public string? SurgerySteps { get; set; }

        [StringLength(500)]
        public string? ActionTaken_Vitrectomy { get; set; }

        public string? Specialinstructions { get; set; }

        public string? Remarks { get; set; }

        public string? Remarksaboutcomplication_undercomplicationmenu { get; set; }

        public string? Other_remarks_outside_complication_menu { get; set; }

        public string? Postopmedications_instructions { get; set; }

        [StringLength(500)]
        public string? Ifanypostopcomplication_Type { get; set; }

        [StringLength(100)]
        public string? DischargeVisionwithpinhole { get; set; }

        public long? Surgeryid { get; set; }

        [StringLength(50)]
        public string? Starttime { get; set; }

        public long? Caseno { get; set; }

        public int? Age { get; set; }

        [StringLength(10)]
        public string? SEX { get; set; }

        [StringLength(100)]
        public string? SurgeonType { get; set; }

        public long? Surgeon { get; set; }

        public string? Patient_comorbidities_systemic { get; set; }

        [StringLength(255)]
        public string? Deptname { get; set; }

        [StringLength(255)]
        public string? Emp { get; set; }

        [StringLength(500)]
        public string? Pre_Operated_eye { get; set; }

        [StringLength(500)]
        public string? PreOthereye { get; set; }

        [StringLength(500)]
        public string? Co_morbidity_diagnosisRE { get; set; }

        [StringLength(500)]
        public string? Co_morbidity_diagnosisLE { get; set; }

        [StringLength(50)]
        public string? Pre_BCVA { get; set; }

        [StringLength(50)]
        public string? Pre_VAwithPG { get; set; }

        [StringLength(50)]
        public string? Pre_UCVA { get; set; }

        [StringLength(500)]
        public string? Ifanypostopcomplication { get; set; }

        [StringLength(50)]
        public string? Ifundergoneanyresurgery { get; set; }

        [StringLength(50)]
        public string? ResurgeryDate { get; set; }

        [StringLength(50)]
        public string? NoofDaysfromprimarysurgery { get; set; }

        [StringLength(100)]
        public string? PinholeVisionafterResurgery { get; set; }

        [StringLength(50)]
        public string? BestcorrectedVisionafterResurgery { get; set; }

        [StringLength(255)]
        public string? Followupadvisedat { get; set; }

        [StringLength(50)]
        public string? FollowupDate { get; set; }

        [StringLength(50)]
        public string? FollowupUCVA { get; set; }

        [StringLength(50)]
        public string? FollowupBCVA { get; set; }

        [StringLength(50)]
        public string? Sphericalequivalent { get; set; }

        public DateTime? InsertedDate { get; set; }
    }
}
