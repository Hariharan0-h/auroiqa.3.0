// models/patient-cataract-qa.model.ts
export interface PatientCataractQa {
  id: number;
  uid?: number;
  patient_NAME?: string;
  peid?: number;
  mr_no?: string;
  surgeryDate?: Date;
  lensname?: string;
  modelname?: string;
  iolPower?: string;
  eye?: string;
  patient_Type?: string;
  alias?: string;
  firstname?: string;
  surgeryType?: string;
  siteid?: number;
  first_advised_date?: Date;
  anesthesia?: string;
  anesthesia_complication?: string;
  listofIntraopIntraopcomplication?: string;
  fromdropdownIntraopIntraopcomplication?: string;
  with_Without_VD?: string;
  surgerySteps?: string;
  actionTaken_Vitrectomy?: string;
  specialinstructions?: string;
  remarks?: string;
  remarksaboutcomplication_undercomplicationmenu?: string;
  other_remarks_outside_complication_menu?: string;
  postopmedications_instructions?: string;
  ifanypostopcomplication_Type?: string;
  dischargeVisionwithpinhole?: string;
  surgeryid?: number;
  starttime?: string;
  caseno?: number;
  age?: number;
  sex?: string;
  surgeonType?: string;
  surgeon?: number;
  patient_comorbidities_systemic?: string;
  deptname?: string;
  emp?: string;
  pre_Operated_eye?: string;
  preOthereye?: string;
  co_morbidity_diagnosisRE?: string;
  co_morbidity_diagnosisLE?: string;
  pre_BCVA?: string;
  pre_VAwithPG?: string;
  pre_UCVA?: string;
  ifanypostopcomplication?: string;
  ifundergoneanyresurgery?: string;
  resurgeryDate?: string;
  noofDaysfromprimarysurgery?: string;
  pinholeVisionafterResurgery?: string;
  bestcorrectedVisionafterResurgery?: string;
  followupadvisedat?: string;
  followupDate?: string;
  followupUCVA?: string;
  followupBCVA?: string;
  sphericalequivalent?: string;
  insertedDate?: Date;
}

// models/patient-glaucoma-qa.model.ts
export interface PatientGlaucomaQa {
  id: number;
  patient_name?: string;
  uid?: number;
  pe_id?: number;
  mr_no?: string;
  surgery_date?: Date;
  lens_name?: string;
  lens_model_name?: string;
  iol_power?: string;
  eye_part?: string;
  patient_type?: string;
  doctor_alias?: string;
  doctor_first_name?: string;
  surgery_type?: string;
  site_id?: number;
  first_advised_date?: Date;
  anesthesia?: string;
  anesthesia_complication?: string;
  intraoperative_complication?: string;
  vd_status?: string;
  surgery_steps?: string;
  action_taken_virectomy?: string;
  complication_remarks?: string;
  complication_remarks_others?: string;
  special_instructions?: string;
  remarks?: string;
  postop_medications_instructions?: string;
  postop_complications?: string;
  discharge_vision_with_pinhole?: string;
  surgery_id?: number;
  surgery_start_time?: string;
  case_no?: number;
  age?: number;
  sex?: string;
  surgeon_type?: string;
  surgeon_id?: number;
  patient_comorbidities_systemic?: string;
  sx_type?: string;
  ble_bleak?: string;
  flap_injury?: string;
  generic_notes?: string;
  complication?: string;
  complication_step?: string;
  complications?: string;
  complication_others?: string;
  devices?: string;
  intracameral?: string;
  iridectomy?: string;
  secondary_procedure?: string;
  side_port_location?: string;
  vitrectomy?: string;
  antimetabolites_level_2?: string;
  antimetabolites_level_3?: string;
  antimetabolites_level_4?: number;
  antimetabolites_level_5?: number;
  pre_diagnosis_re?: string;
  pre_diagnosis_le?: string;
  co_morbidity_diagnosis_re?: string;
  co_morbidity_diagnosis_le?: string;
  pre_bcva?: string;
  pre_va_with_pg?: string;
  pre_ucva?: string;
  ifanypostopcomplication?: string;
  undergone_resurgery?: string;
  resurgery_date?: Date;
  no_of_days_form_primary_surgery?: number;
  pinhole_vision_after_resurgery?: string;
  bcva_after_resurgery?: string;
  followup_advised_at?: string;
  followup_date?: Date;
  followup_ucva?: string;
  followup_bcva?: string;
  spherical_equivalent?: string;
  follow_up_advise?: string;
  followup_compliance?: string;
  postop_ucva?: string;
  postop_bcva?: string;
  next_visit_date?: string;
  actual_visit_date?: string;
  actual_site_name?: string;
  post_operative_iop?: number;
  pre_operative_iop?: number;
  combined?: string;
  is_active?: boolean;
  pre_op_bcva_id?: number;
  others?: number;
  post_op_ucva_id?: number;
  post_op_bcva_id?: number;
  followup_bcva_id?: number;
  followup_ucva_id?: number;
}

// models/api-response.model.ts
export interface ApiResponse<T> {
  data: T;
  totalCount: number;
  page: number;
  pageSize: number;
}

export interface StatsResponse {
  totalRecords: number;
  totalPatients: number;
  surgeryTypeStats: Array<{ surgeryType: string; count: number }>;
  eyeStats?: Array<{ eye: string; count: number }>;
  eyePartStats?: Array<{ eyePart: string; count: number }>;
  surgeonTypeStats?: Array<{ surgeonType: string; count: number }>;
  recordsWithComplications?: number;
  activeRecords?: number;
  inactiveRecords?: number;
}

export interface IOPAnalysisResponse {
  totalRecordsWithIOP: number;
  averagePreOperativeIOP: number;
  averagePostOperativeIOP: number;
  averageIOPReduction: number;
  iopData: Array<{
    id: number;
    patient_name: string;
    mr_no: string;
    surgery_date: Date;
    preOpIOP: number;
    postOpIOP: number;
    iopReduction: number;
  }>;
}

export interface ComplicationsResponse {
  postOperativeComplications: Array<{ complicationType: string; count: number }>;
  intraOperativeComplications: Array<{ complicationType: string; count: number }>;
  anesthesiaComplications: Array<{ complicationType: string; count: number }>;
}