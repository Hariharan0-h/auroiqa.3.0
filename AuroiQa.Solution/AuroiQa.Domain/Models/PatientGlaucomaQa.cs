using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuroiQa.Domain.Models
{
    [Table("patient_glaucoma_qa")]
    public class PatientGlaucomaQa
    {
        [Key]
        public long Id { get; set; }

        [StringLength(255)]
        public string? Patient_name { get; set; }

        public long? Uid { get; set; }

        public long? Pe_id { get; set; }

        [StringLength(50)]
        public string? Mr_no { get; set; }

        public DateTime? Surgery_date { get; set; }

        [StringLength(255)]
        public string? Lens_name { get; set; }

        [StringLength(255)]
        public string? Lens_model_name { get; set; }

        [StringLength(50)]
        public string? Iol_power { get; set; }

        [StringLength(50)]
        public string? Eye_part { get; set; }

        [StringLength(100)]
        public string? Patient_type { get; set; }

        [StringLength(100)]
        public string? Doctor_alias { get; set; }

        [StringLength(100)]
        public string? Doctor_first_name { get; set; }

        [StringLength(255)]
        public string? Surgery_type { get; set; }

        public long? Site_id { get; set; }

        public DateTime? First_advised_date { get; set; }

        [StringLength(255)]
        public string? Anesthesia { get; set; }

        [StringLength(500)]
        public string? Anesthesia_complication { get; set; }

        [StringLength(500)]
        public string? Intraoperative_complication { get; set; }

        [StringLength(100)]
        public string? Vd_status { get; set; }

        public string? Surgery_steps { get; set; }

        [StringLength(500)]
        public string? Action_taken_virectomy { get; set; }

        public string? Complication_remarks { get; set; }

        public string? Complication_remarks_others { get; set; }

        public string? Special_instructions { get; set; }

        public string? Remarks { get; set; }

        public string? Postop_medications_instructions { get; set; }

        [StringLength(500)]
        public string? Postop_complications { get; set; }

        [StringLength(100)]
        public string? Discharge_vision_with_pinhole { get; set; }

        public long? Surgery_id { get; set; }

        [StringLength(50)]
        public string? Surgery_start_time { get; set; }

        public long? Case_no { get; set; }

        public int? Age { get; set; }

        [StringLength(10)]
        public string? Sex { get; set; }

        [StringLength(100)]
        public string? Surgeon_type { get; set; }

        public long? Surgeon_id { get; set; }

        public string? Patient_comorbidities_systemic { get; set; }

        [StringLength(100)]
        public string? Sx_type { get; set; }

        [StringLength(100)]
        public string? Ble_bleak { get; set; }

        [StringLength(255)]
        public string? Flap_injury { get; set; }

        public string? Generic_notes { get; set; }

        [StringLength(255)]
        public string? Complication { get; set; }

        [StringLength(255)]
        public string? Complication_step { get; set; }

        public string? Complications { get; set; }

        [StringLength(500)]
        public string? Complication_others { get; set; }

        [StringLength(255)]
        public string? Devices { get; set; }

        [StringLength(255)]
        public string? Intracameral { get; set; }

        [StringLength(255)]
        public string? Iridectomy { get; set; }

        [StringLength(255)]
        public string? Secondary_procedure { get; set; }

        [StringLength(100)]
        public string? Side_port_location { get; set; }

        [StringLength(255)]
        public string? Vitrectomy { get; set; }

        [StringLength(255)]
        public string? Antimetabolites_level_2 { get; set; }

        [StringLength(255)]
        public string? Antimetabolites_level_3 { get; set; }

        [Column(TypeName = "decimal(10,4)")]
        public decimal? Antimetabolites_level_4 { get; set; }

        [Column(TypeName = "decimal(10,4)")]
        public decimal? Antimetabolites_level_5 { get; set; }

        [StringLength(500)]
        public string? Pre_diagnosis_re { get; set; }

        [StringLength(500)]
        public string? Pre_diagnosis_le { get; set; }

        [StringLength(500)]
        public string? Co_morbidity_diagnosis_re { get; set; }

        [StringLength(500)]
        public string? Co_morbidity_diagnosis_le { get; set; }

        [StringLength(50)]
        public string? Pre_bcva { get; set; }

        [StringLength(50)]
        public string? Pre_va_with_pg { get; set; }

        [StringLength(50)]
        public string? Pre_ucva { get; set; }

        [StringLength(500)]
        public string? Ifanypostopcomplication { get; set; }

        [StringLength(50)]
        public string? Undergone_resurgery { get; set; }

        public DateTime? Resurgery_date { get; set; }

        public int? No_of_days_form_primary_surgery { get; set; }

        [StringLength(100)]
        public string? Pinhole_vision_after_resurgery { get; set; }

        [StringLength(50)]
        public string? Bcva_after_resurgery { get; set; }

        [StringLength(255)]
        public string? Followup_advised_at { get; set; }

        public DateTime? Followup_date { get; set; }

        [StringLength(50)]
        public string? Followup_ucva { get; set; }

        [StringLength(50)]
        public string? Followup_bcva { get; set; }

        [StringLength(50)]
        public string? Spherical_equivalent { get; set; }

        public string? Follow_up_advise { get; set; }

        [StringLength(255)]
        public string? Followup_compliance { get; set; }

        [StringLength(50)]
        public string? Postop_ucva { get; set; }

        [StringLength(50)]
        public string? Postop_bcva { get; set; }

        [StringLength(50)]
        public string? Next_visit_date { get; set; }

        [StringLength(50)]
        public string? Actual_visit_date { get; set; }

        [StringLength(255)]
        public string? Actual_site_name { get; set; }

        public int? Post_operative_iop { get; set; }

        public int? Pre_operative_iop { get; set; }

        [StringLength(255)]
        public string? Combined { get; set; }

        public bool? Is_active { get; set; }

        public int? Pre_op_bcva_id { get; set; }

        public int? Others { get; set; }

        public int? Post_op_ucva_id { get; set; }

        public int? Post_op_bcva_id { get; set; }

        public int? Followup_bcva_id { get; set; }

        public int? Followup_ucva_id { get; set; }
    }
}
