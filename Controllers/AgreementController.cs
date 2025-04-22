using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Agreement.Models;
using Agreement.Data;
using System.Diagnostics;
using Agreement.Services;
using DinkToPdf;
using System.Text;

namespace Agreement.Controllers
{
    public class AgreementController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly IWebHostEnvironment _env;
        private readonly PdfGenerationService _pdfService;

        // Updated constructor with all dependencies
        public AgreementController(
            AppDbContext context,
            IConfiguration config,
            IWebHostEnvironment env,
            PdfGenerationService pdfService)
        {
            _context = context;
            _config = config;
            _env = env;
            _pdfService = pdfService;
        }

        // First Form (File Uploads)
        public IActionResult Index() => View();

        // Second Form (With Signature)
        public IActionResult Index2(int id)
        {
            var agreement = _context.Agreements.Find(id);
            if (agreement == null)
            {
                return NotFound();
            }
            return View(agreement);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index2(int id, AgreementRecord record)
        {
            if (id != record.Id)
            {
                return NotFound();
            }

            try
            {
                var existingAgreement = await _context.Agreements.FindAsync(id);
                if (existingAgreement == null)
                {
                    return NotFound();
                }

                // Update only the signature-related fields
                existingAgreement.SignatureData = record.SignatureData;
                existingAgreement.SignedDate = DateTime.Now;
                existingAgreement.drfullname = record.drfullname;

                _context.Update(existingAgreement);
                await _context.SaveChangesAsync();

                // Generate and store PDF before redirect
                var pdfBytes = GenerateAgreementPdf(existingAgreement);
                HttpContext.Session.Set("AgreementPdf", pdfBytes);

                return RedirectToAction("ThankYou", new { id = record.Id });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving signature: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the signature.");
                return View(record);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Create(AgreementRecord record)
        {
            var uploadPath = _config["FileUploadPath"] ?? Path.Combine(_env.WebRootPath, "uploads");
            Directory.CreateDirectory(uploadPath);

            // Process all file uploads
            if (record.hpcsa != null)
            {
                record.hpcsafile = record.hpcsa.FileName;
                record.hpcsafileStoredName = $"{Guid.NewGuid()}{Path.GetExtension(record.hpcsa.FileName)}";
                await SaveFile(record.hpcsa, Path.Combine(uploadPath, record.hpcsafileStoredName));
            }

            if (record.boh != null)
            {
                record.bohffile = record.boh.FileName;
                record.bohffileStoredname = $"{Guid.NewGuid()}{Path.GetExtension(record.boh.FileName)}";
                await SaveFile(record.boh, Path.Combine(uploadPath, record.bohffileStoredname));
            }

            if (record.ppi != null)
            {
                record.ppiifile = record.ppi.FileName;
                record.ppiifileStoredName = $"{Guid.NewGuid()}{Path.GetExtension(record.ppi.FileName)}";
                await SaveFile(record.ppi, Path.Combine(uploadPath, record.ppiifileStoredName));
            }

            if (record.idf != null)
            {
                record.idfile = record.idf.FileName;
                record.idfileStoredName = $"{Guid.NewGuid()}{Path.GetExtension(record.idf.FileName)}";
                await SaveFile(record.idf, Path.Combine(uploadPath, record.idfileStoredName));
            }

            if (record.qsf != null)
            {
                record.qsfile = record.qsf.FileName;
                record.qsfileStoredName = $"{Guid.NewGuid()}{Path.GetExtension(record.qsf.FileName)}";
                await SaveFile(record.qsf, Path.Combine(uploadPath, record.qsfileStoredName));
            }

            if (record.emer != null)
            {
                record.emerfile = record.emer.FileName;
                record.emerfileStoredName = $"{Guid.NewGuid()}{Path.GetExtension(record.emer.FileName)}";
                await SaveFile(record.emer, Path.Combine(uploadPath, record.emerfileStoredName));
            }

            try
            {
                _context.Agreements.Add(record);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index2", new { id = record.Id });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error saving agreement: {ex.Message}");
                ModelState.AddModelError("", "An error occurred while saving the data.");
                return View("Index", record);
            }
        }

        public async Task<IActionResult> DownloadFile(string storedName)
        {
            var uploadPath = _config["FileUploadPath"] ?? Path.Combine(_env.WebRootPath, "uploads");
            var filePath = Path.Combine(uploadPath, storedName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var memory = new MemoryStream();
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                await stream.CopyToAsync(memory);
            }
            memory.Position = 0;

            var contentType = "application/octet-stream";
            var ext = Path.GetExtension(filePath).ToLowerInvariant();
            switch (ext)
            {
                case ".pdf": contentType = "application/pdf"; break;
                case ".doc": contentType = "application/msword"; break;
                case ".docx": contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document"; break;
                case ".xls": contentType = "application/vnd.ms-excel"; break;
                case ".xlsx": contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"; break;
                case ".txt": contentType = "text/plain"; break;
            }

            return File(memory, contentType, storedName);
        }

        public IActionResult ThankYou(int id)
        {
            var agreement = _context.Agreements.Find(id);
            if (agreement == null)
            {
                return NotFound();
            }
            return View(agreement);
        }

        public IActionResult DownloadAgreementPdf()
        {
            if (HttpContext.Session.TryGetValue("AgreementPdf", out var pdfBytes))
            {
                return File(pdfBytes, "application/pdf", "Agreement.pdf");
            }
            return NotFound();
        }

        private byte[] GenerateAgreementPdf(AgreementRecord agreement)
        {
            var pdfHtml = new StringBuilder();
            pdfHtml.AppendLine("<html><head><style>");
            pdfHtml.AppendLine("body { font-family: Arial; margin: 2cm; }");
            pdfHtml.AppendLine("h1 { color: #0066cc; }");
            pdfHtml.AppendLine(".signature { margin-top: 3cm; }");
            pdfHtml.AppendLine("</style></head><body>");

            pdfHtml.AppendLine("<h1>Agreement Confirmation</h1>");
            pdfHtml.AppendLine($"<p><strong>Doctor's Name & Surname:</strong> {agreement.drname} {agreement.drsurname}</p>");
            pdfHtml.AppendLine($"<p><strong>ID NUMBER:</strong> {agreement.dridnr}</p>");
            pdfHtml.AppendLine($"<p><strong>BHF NUMBER:</strong> {agreement.bhff}</p>");
            pdfHtml.AppendLine($"<p><strong>ADDRESS:</strong> {agreement.draddress}</p>");
            pdfHtml.AppendLine($"<p><b>[\"hereinafter \"the Medical Practitioner\"]</b></p>");
            pdfHtml.AppendLine($"<p>and</p>");
            pdfHtml.AppendLine($"<p><b>MELOMED HOSPITAL HOLDINGS (PTY) LTD</b></p>");
            pdfHtml.AppendLine($"<p>(Registration No: 1998/001843/07)</p>");
            pdfHtml.AppendLine($"<p>of</p>");
            pdfHtml.AppendLine($"<p>Unit 6 and 8, Melomed Office Park, Punters Way, Kenilworth</p>");
            pdfHtml.AppendLine($"<p>[herein represented by <b>ISMAIL EBRAHIM BHORAT</b>, a Director duly authorised to act herein]</p>");
            pdfHtml.AppendLine($"<p></p>");
            pdfHtml.AppendLine($"<p><b>IT IS HEREBY AGREED THAT:</b></p>");
            pdfHtml.AppendLine($"<p><b>1. PURPOSE</b></p>");
            pdfHtml.AppendLine($"<p>1.1. The purpose this Agreement is to establish the Admission Privileges and code of conduct referred to in paragraph 6.11 below, for the Medical Practitioner to apply for and practice within the Hospital.</p>");
            pdfHtml.AppendLine($"<p><b>2. THE SCOPE</b></p>");
            pdfHtml.AppendLine($"<p>2.1. This Agreement also extends to any registered medical practitioner that wishes to utilise the Hospital for any care, treatment, and rehabilitation of and for any of his or her Patients, whilst admitted as an inpatient.</p>");
            pdfHtml.AppendLine($"<p><b>3. INTERPRETATION</b></p>");
            pdfHtml.AppendLine($"<p>3.1. In this Agreement, except in a context indicating that some other meaning is intended:</p>");
            pdfHtml.AppendLine($"<p>3.1.1. <b>“Agreement”</b> means this Admission Privileges Agreement, subject to the terms and conditions contained herein.</b></p>");
            pdfHtml.AppendLine($"<p>3.1.2. <b>&quot;Admission Privileges&quot;</b> means the approved and authorised rights of the Medical Practitioner to admit, care and / or treat any Patient at the Hospital under this Agreement.</p>");
            pdfHtml.AppendLine($"<p>3.1.3. <b>&quot;the Medical Practitioner&quot;</b> means the Medical Practitioner as referred to on page 1, providing health services in terms of any law, including but not limited to:</p>");
            pdfHtml.AppendLine($"<p>3.1.3.1. Allied Health Professions Act, 1982 (Act No. 63 of 1982);</p>");
            pdfHtml.AppendLine($"<p>3.1.3.2. Health Professions Act, 1974 (Act No. 56 of 1974);</p>");
            pdfHtml.AppendLine($"<p>3.1.3.3. Nursing Act, 1978 (Act No. 50 of 1978);</p>");
            pdfHtml.AppendLine($"<p>3.1.3.4. Pharmacy Act. 1974 (Act No. 53 of 1974);</p>");
            pdfHtml.AppendLine($"<p>3.1.3.5. Dental Technicians Act, 1979 (Act No. 19 of 1979) and;</p>");
            pdfHtml.AppendLine($"<p>3.1.3.6. The Health Professions Council of South Africa (&quot;HPCSA&quot;).</p>");
            pdfHtml.AppendLine($"<p>3.1.4. <b>&quot;Melomed Hospital Holdings (Pty) Ltd&quot;</b> (Registration No.: 1998/001843/07) means the holding company, its subsidiaries and related parties, where the Medical Practitioner will be practicing from time to time in terms of this Agreement, specifically including the following:</p>");
            pdfHtml.AppendLine($"<p>3.1.4.1. Gatesville Medical Centre (Pty) Ltd (Registration No.: 1987/002357/07) t/a Melomed Gatesville, with its property situated on the property Erf 110766, Athlone, and any improvements on it;</p>");
            pdfHtml.AppendLine($"<p>3.1.4.2. Mitchells Plain Medical Centre (Pty) Ltd (Registration No.: 1997/019988/07) t/a Melomed Mitchells Plain, with its property situated on the property Erf 29388, Mitchells Plain, and any improvements on it;</p>");
            pdfHtml.AppendLine($"<p>3.1.4.3. Melovest Investment Holdings (Pty) Ltd (Registration No.: 1998/002486/07) t/a Melomed Bellville, with its property situated on the property Erf 39912, Bellville, and any improvements on it;</p>");
            pdfHtml.AppendLine($"<p>3.1.4.4. Melomed Claremont Hospital (Pty) Ltd (Registration No.: 2007/018468/07) t/a Melomed Tokai, with its property situated on Erf 175146, Cape Town, and any improvements on it;</p>");
            pdfHtml.AppendLine($"<p>3.1.4.5. Melomed Ambulance Services (Pty) Ltd (Registration No.: 2003/023220/07);</p>");
            pdfHtml.AppendLine($"<p>3.1.4.6. Melomed Private Clinic (Pty) Ltd (Registration No.: 2014/005415/07) t/a Melomed Private Clinic, with its property situated on Erf 52308, Cape Town, and any improvements on it; and</p>");
            pdfHtml.AppendLine($"<p>3.1.4.7. Melomed Richards Bay (Pty) Ltd (Registration No.: 2014/034552/07) t/a Melomed Richards Bay, with its property situated on the remaining portion 19 of 16, Erf 11451, Richards Bay, and any improvements on it.</p>");
            pdfHtml.AppendLine($"<p>(where applicable, the individual hospital where the Medical Practitioner will be practicing and referred to above in paragraph 3.1.4.1 – 3.1.4.7, is hereinafter referred to as <b>&ldquothe Hospital&quot;</b>)</p>");
            pdfHtml.AppendLine($"<p>3.1.5. <b>&ldquothe Melomed Group&quot;</b> means the Hospitals, the ambulance services and pharmacies, which are subsidiaries of Melomed Hospital Holdings (Pty) Ltd or who work in association with the Hospitals and/or which form an integral part of the services rendered by the Hospitals within the Melomed Group.</p>");
            pdfHtml.AppendLine($"<p>3.1.6. <b>&ldquoProfessional Indemnity Insurance&quot;</b> means adequate professional liability or indemnity insurance for the Medical Practitioner to provide healthcare services, advice or treatment at the Hospital, and which insures the Medical Practitioner for any act, omission and /or breach of his or her professional duty during the course of his or her Admission Privileges at the Hospital.</p>");
            pdfHtml.AppendLine($"<p>3.1.7. <b>&ldquoPatient or User&quot;</b> means the person receiving treatment in the Hospital, both admitted or within the premises of the Hospital, including receiving blood or blood products, or using a health service, and if the person receiving treatment or using a health service is:</p>");
            pdfHtml.AppendLine($"<p>3.1.7.1. below the age of 12 years as contemplated in the Children’s Act, 2003, “User” includes the person’s parent or guardian or another person authorised by law to act on the first mentioned person’s behalf; or</p>");
            pdfHtml.AppendLine($"<p>3.1.7.2. incapable of taking decisions, &quot;User&quot; includes the person’s spouse or partner or, in the absence of such spouse or partner, the person’s parent, grandparent, adult child or brother or sister, or another person authorised by law to act on the first mentioned person’s behalf.</p>");
            pdfHtml.AppendLine($"<p>3.1.8. <b>&quot;the Parties&quot;</b> means the parties to this Agreement, and “party” means one of them;</p>");
            pdfHtml.AppendLine($"<p>3.2. Any provision of this Agreement imposing a restraint, prohibition or restriction on the Medical Practitioner shall be so construed that the Medical Practitioner is not only bound to comply therewith but is also obliged to procure that the same restraint, prohibition or restriction is observed by everybody occupying or entering the Hospital or any part thereof through, under, by arrangement with, or at the invitation of, the Medical Practitioner, including (without limiting the generality of this provision) its Associates and the directors, members, officers, employees, agents, clients and invitees of the Medical Practitioner or its Associates.</p>");
            pdfHtml.AppendLine($"<p>3.3. Clause headings appear in this Agreement for purposes of reference only and shall not influence the proper interpretation of the subject matter.</p>");
            pdfHtml.AppendLine($"<p>3.4. This Agreement shall be interpreted and applied in accordance with South African law.</p>");
            pdfHtml.AppendLine($"<p><b>4. OBJECTIVES</b></p>");
            pdfHtml.AppendLine($"<p>4.1. The objective of this Agreement is to maintain and promote professional standards, ethical behaviour and to provide quality private healthcare services at the Hospital, ensuring that all Parties:</p>");
            pdfHtml.AppendLine($"<p>4.1.1. Respect the dignity of all persons, patients, staff, management, visitors and stakeholders at the Hospital.</p>");
            pdfHtml.AppendLine($"<p>4.1.2. Complies with all standards set out by the relevant regulatory bodies.</p>");
            pdfHtml.AppendLine($"<p>4.1.3. Ensure an efficient and transparent process for managing incidents, as well as respecting and maintaining the reputation of the Hospital, its owners, directors, management, staff, service providers and associated health professionals.</p>");
            pdfHtml.AppendLine($"<p><b>5. THE AGREEMENT PERIOD</b></p>");
            pdfHtml.AppendLine($"<p>5.1 The Agreement shall come into operation on the on 1 June 2024 and shall subsist for an indefinite period.</p>");
            pdfHtml.AppendLine($"<p>5.2 The parties agree that either party may terminate this Agreement with immediate effect upon furnishing the other party with notice to their domicilia citandi et executandi.</p>");
            pdfHtml.AppendLine($"<p><b>6. ADMISSION PRIVILEGES OF THE MEDICAL PRACTITIONER</b></p>");
            pdfHtml.AppendLine($"<p>6.1. The Hospital affords the Medical Practitioner the privilege of admitting and/or treating Patients to and at the Hospital and facilities provided by the Hospital.</p>");
            pdfHtml.AppendLine($"<p>6.2. In the event of the discharge of a Patient who has been referred to the Medical Practitioner by another doctor, the Medical Practitioner shall refer the Patient back to the referring doctor, together with appropriate feedback to the referring specialist.</p>");
            pdfHtml.AppendLine($"<p>6.3. The Hospital requires an adequately completed medical record that enables both the Medical Practitioner and a third party to reconstruct a consultation, or treatment, without reference to the Medical Practitioner’s memory.</p>");
            pdfHtml.AppendLine($"<p>6.4. The Medical Practitioner shall accordingly ensure that all clinical/medical records completed by him/her shall be comprehensive and clearly legible, shall provide dates and times when consultations and/or treatments and/or procedures took place, shall be signed by him/her and completed simultaneously. Abbreviations should be avoided and alterations or additions shall be initialled by the Medical Practitioner.</p>");
            pdfHtml.AppendLine($"<p>6.5. The Medical Practitioner shall inform all general practitioners who refer Patients to him/her of progress relating to the treatment of such Patients, so that the general practitioner concerned is kept updated on any development in the condition of the Patient, or the treatment of such Patient. Melomed has made available an app called Medchat to assist in this regard.</p>");
            pdfHtml.AppendLine($"<p>6.6. The Medical Practitioner must timeously and diligently adhere to all appropriate clinical guidelines to treat, care and / or rehabilitate the Patients. This includes after-hours consultation, treating emergency medical conditions and when not available, making alternative arrangements with the Hospital for the Patient.</p>");
            pdfHtml.AppendLine($"<p>6.7. The Medical Practitioner shall ensure that the Patient’s informed consent is obtained, in writing, in compliance with all laws and ethical guidelines applicable from time to time and guidelines as specified by regulatory authorities and insurance requirements, in respect of all instructions, admissions and treatment that require same.</p>");
            pdfHtml.AppendLine($"<p>6.8. The Medical Practitioner must familiarise themselves and adhere to the internal policies of the Hospital, including but not limited to:</p>");
            pdfHtml.AppendLine($"<p>6.8.1. Patient and the Hospital’s privacy and confidentiality rights, as well as internal protocols for the protection of confidential information and intellectual property.</p>");
            pdfHtml.AppendLine($"<p>6.8.2. Informed consent.</p>");
            pdfHtml.AppendLine($"<p>6.8.3. Infection control measures, standards and criteria as implemented within the Hospital.</p>");
            pdfHtml.AppendLine($"<p>6.8.4. Internal admission procedures, requirements and the rules applicable to the Hospital, including the Medical Practitioner making arrangements to organise an anaesthetist to assist in surgery and have an assistant, as and when needed.</p>");
            pdfHtml.AppendLine($"<p>6.8.5. Health and safety requirements as established under applicable law, as well as measures undertaken by the Hospital to ensure compliance with such.</p>");
            pdfHtml.AppendLine($"<p>6.8.6. The Hospital protocols, policies and conditions of service in so far as they apply to employees of the Hospital, and not to interfere, diminish or undermine the Hospital’s employment relationship with said employees.</p>");
            pdfHtml.AppendLine($"<p>6.8.7. Protocols and applicable law, pertaining to the reporting of unnatural deaths.</p>");
            pdfHtml.AppendLine($"<p>6.8.8. The provision of adequate, accurate and detailed clinical notes and information to the Hospital’s employees, and compliance with the relevant laws pertaining to medicine and requirements of medical aids and manage care.</p>");
            pdfHtml.AppendLine($"<p>6.8.9. To conduct him/herself at all times in a manner compliant with his/her ethical code, and registered scope of practice.</p>");
            pdfHtml.AppendLine($"<p>6.8.10. To provide letters of motivation as requested by the Hospital, who in turn request same at the request of Funders.</p>");
            pdfHtml.AppendLine($"<p>6.8.11. Practice only within the scope of his/her speciality, as governed by the HPCSA.</p>");
            pdfHtml.AppendLine($"<p>6.9. The Medical Practitioner is expected to attend all formal meetings, including but not limited to the Antimicrobial Stewardship meeting, as well as M&amp;M meetings arranged by the Hospital’s management and doctors practicing at</p>");
            pdfHtml.AppendLine($"<p>6.10. The Medical Practitioner must furnish, where applicable and as renewed, certified copies of the following documents to the Hospital at the commencement of this Agreement:</p>");
            pdfHtml.AppendLine($"<p>6.10.1. Health Professional Council of South Africa certificate;</p>");
            pdfHtml.AppendLine($"<p><strong>6.10.1. Health Professional Council of South Africa certificate: </strong> {agreement.hpcsafileStoredName}</p>");
            pdfHtml.AppendLine($"<p><strong>EXPIRY DATE: </strong> {agreement.hpcsaexpire}</p>");
            pdfHtml.AppendLine($"<p><strong>6.10.2. Board of Health Funders certificate;: </strong> {agreement.bohffileStoredname}</p>");
            pdfHtml.AppendLine($"<p><strong>EXPIRY DATE: </strong> {agreement.bohfexpire}</p>");
            pdfHtml.AppendLine($"<p><strong>6.10.3. Proof of Professional Indemnity Insurance with appropriate cover over and above R15,000,000.00 (fifteen million Rands);: </strong> {agreement.ppiifileStoredName}</p>");
            pdfHtml.AppendLine($"<p><strong>EXPIRY DATE: </strong> {agreement.ppiiexpire}</p>");
            pdfHtml.AppendLine($"<p><strong>6.10.4. Identity document: </strong> {agreement.idfileStoredName}</p>");
            pdfHtml.AppendLine($"<p><strong>6.10.5. All relevant qualifications, as well as specialisations (if applicable): </strong> {agreement.qsfileStoredName}</p>");
            pdfHtml.AppendLine($"<p><strong>6.10.6. If application is made in regard to any specific discipline, such as Casualty/Emergency Medication, then copies of the prescribed qualifications (and registration certificates): </strong> {agreement.emerfileStoredName}</p>");
            pdfHtml.AppendLine($"<p>6.10.7. Contact details.</p>");
            pdfHtml.AppendLine($"<p><strong>CELLPHONE NUMBER: </strong> {agreement.drcell}</p>");
            pdfHtml.AppendLine($"<p><strong>EMAIL ADRESS: </strong> {agreement.dremail}</p>");
            pdfHtml.AppendLine($"<p>PHYSICAL ADDRESS</p>");
            pdfHtml.AppendLine($"<p>{agreement.drphysicaddrs}</p>");
            pdfHtml.AppendLine($"<p>6.11. The Hospital deems the following conduct as unacceptable during the Medical Practitioner’s Admission Privileges, which includes but is not limited to:</p>");
            pdfHtml.AppendLine($"<p>6.11.1. Conduct breaching HPCSA regulations or guidelines;</p>");
            pdfHtml.AppendLine($"<p>6.11.2. Putting the safety of any person at the Hospital at risk;</p>");
            pdfHtml.AppendLine($"<p>6.11.3. Damage to the Hospital equipment or property;</p>");
            pdfHtml.AppendLine($"<p>6.11.4. Gross mismanagement or neglect of patient care;</p>");
            pdfHtml.AppendLine($"<p>6.11.5. Verbally abusing, threatening or insulting any member of staff, patient or visitor to the health establishment;</p>");
            pdfHtml.AppendLine($"<p>6.11.6. Any form of unlawful discrimination;</p>");
            pdfHtml.AppendLine($"<p>6.11.7. Assault or the threat thereof;</p>");
            pdfHtml.AppendLine($"<p>6.11.8. Sexual harassment;</p>");
            pdfHtml.AppendLine($"<p>6.11.9. Being under the influence of any unlawful or intoxicating substance whilst on duty, at the facility or on call.</p>");
            pdfHtml.AppendLine($"<p>6.12. The Medical Practitioner places their name on the call roster of the Hospital as and when required by the Hospital and shall:</p>");
            pdfHtml.AppendLine($"<p>6.12.1. ensure that when on call, he/she will, at all times, be contactable by cellular telephone.</p>");
            pdfHtml.AppendLine($"<p>6.12.2. if called, will immediately be available to attend at the Hospital in terms of any call received.</p>");
            pdfHtml.AppendLine($"<p>6.12.3. if the Medical Practitioner is unable to be on call, he/she shall personally communicate with the Hospital manager, prior to the commencement of the on-call period, and arrange for an alternative Medical Practitioner of the same speciality to be on call for the period.</p>");
            pdfHtml.AppendLine($"<p>6.12.4. If the Hospital’s manager does not have the contact details of the alternative Medical Practitioner concerned, he/she shall notify the Hospital Manager of his/her non-availability and of the identity and contact details of his/her replacement Medical Practitioner.</p>");
            pdfHtml.AppendLine($"<p><b>7. MAINTENANCE AND REPAIRS</b></p>");
            pdfHtml.AppendLine($"<p>7.1. Upon the Hospital proving the Medical Practitioners negligence, the Medical Practitioner shall replace, repair or remedy all damages to all of the Hospital’s property, that was caused as a result of the Medical Practitioners conduct, omission and / or negligence. This includes hospital equipment utilised within theatre that has been damaged.</p>");
            pdfHtml.AppendLine($"<p>7.2. Should the Medical Practitioner’s proven negligence not be remedied under clause 7.1, the Hospital shall be entitled, without prejudice to any of its other rights or remedies, to recover the cost thereof from the Medical Practitioner.</p>");
            pdfHtml.AppendLine($"<p><b>8. DOMICILIA AND NOTICES</b></p>");
            pdfHtml.AppendLine($"<p>8.1. The parties choose as their domicilia citandi et executandi the addresses mentioned in clause 8.1.1 and 8.1.2 below, provided that such domicilium of either party may be changed by written notice from such party to the other party with effect from the date of receipt or deemed receipt by the latter of such notice.</p>");
            pdfHtml.AppendLine($"<p>8.1.1. The Hospital:</p>");
            pdfHtml.AppendLine($"<p>Physical Address:</p>");
            pdfHtml.AppendLine($"<p><b>Melomed Hospital Holdings, Unit 6 and 8 Melomed Office Park, Punters Way, Kenilworth, Western Cape.</b></p>");
            pdfHtml.AppendLine($"<p>Email Address:</p>");
            pdfHtml.AppendLine($"<p><b>legal@melomed.co.za</b></p>");
            pdfHtml.AppendLine($"<p>8.1.2. The Medical Practitioner:</p>");
            pdfHtml.AppendLine($"<p>Physical Address: <b>His/Her residential address as it appears on the first page of this Agreement, alternatively, the Hospital.</b></p>");
            pdfHtml.AppendLine($"<p><strong>EMAIL NUMBER: </strong> {agreement.physemail}</p>");
            pdfHtml.AppendLine($"<p>8.2. Any notice, acceptance, demand or other communication properly addressed by either party to the other party at the latter’s domicilium in terms hereof for the time being and sent</p>");
            pdfHtml.AppendLine($"<p><b>9. WHOLE AGREEMENT</b></p>");
            pdfHtml.AppendLine($"<p>9.1. This is the entire agreement between the parties.</p>");
            pdfHtml.AppendLine($"<p>9.2. Neither party relies in entering into this agreement on any warranties, representations, disclosures or expressions of opinion which have not been incorporated into this agreement as warranties or undertakings.</p>");
            pdfHtml.AppendLine($"<p>9.3. No variation or consensual cancellation of this agreement shall be of any force or effect unless reduced to writing and signed by both parties.</p>");
            pdfHtml.AppendLine($"<p><b>10. NON-WAIVER</b></p>");
            pdfHtml.AppendLine($"<p>10.1. Neither party shall be regarded as having waived, or be precluded in any way from exercising, any right under or arising from this Agreement by reason of such party having at any time granted any extension of time for, or having shown any indulgence to, the other party with reference to any payment or performance hereunder, or having failed to enforce, or delayed in the enforcement of, any right of action against the other party.</p>");
            pdfHtml.AppendLine($"<p>10.2. The failure of either party to comply with any non-material provision of this Agreement shall not excuse the other party from performing the latter’s obligations hereunder fully and timeously.</p>");
            pdfHtml.AppendLine($"<p><b>11. WARRANTY OF AUTHORITY</b></p>");
            pdfHtml.AppendLine($"<p>The persons signing this Agreement on behalf of the parties expressly warrant their authority to do so.</p>");
            pdfHtml.AppendLine($"<p><b>12. SEVERABILITY</b></p>");
            pdfHtml.AppendLine($"<p>12.1. If any court or competent authority finds that any provision of this agreement (or part of any provision) is invalid, illegal or unenforceable, that provision or part-provision shall, to the extent required, be deemed to be deleted, and the validity and enforceability of the other provisions of this agreement shall not be affected.</p>");
            pdfHtml.AppendLine($"<p>12.2. If any invalid, unenforceable or illegal provision of this agreement would be valid, enforceable and legal if some part of it were deleted, the parties shall negotiate in good faith to amend such provision such that, as amended, it is legal, valid and enforceable, and, to the greatest extent possible, achieves the parties’ original commercial intention.</p>");
            pdfHtml.AppendLine($"<p><b>13. CONSENT TO JURISDICTION</b></p>");
            pdfHtml.AppendLine($"<p>13.1. In the event of any legal proceedings in connection with this agreement, the parties hereby consent, in terms of Section 45 (1) of the Magistrate’s Court Act No. 32 of 1944, to the jurisdiction of the Wynberg District Magistrate’s Court and / or the Wynberg Regional Magistrate Court, which shall have jurisdiction under Section 28 (1) of the said Act.</p>");
            pdfHtml.AppendLine($"<p>13.2. In the event of any legal proceedings in connection with this agreement, the parties hereby further consent to the High Court of South Africa, Western Cape Division, Cape Town.</p>");
            pdfHtml.AppendLine($"<p>13.3. The Medical Practitioner hereby agrees and consents that the Hospital shall, at its absolute discretion, be entitled to elect any of the Courts referred to in paragraph 13.1.1 and 13.1.2, to institute legal proceedings in connection with this agreement.</p>");
            pdfHtml.AppendLine($"<p>13.4. The parties hereby agree that the legal proceedings referred to in paragraph 13 is instituted on the basis that this agreement was concluded at the Hospital’s domicilia citandi et executandi address referred to in paragraph 8.1.1, Melomed Hospital Holdings (Pty) Ltd head office.</p>");

            pdfHtml.AppendLine($"<p></p>");
            pdfHtml.AppendLine($"<p><strong>Signed Date:</strong> {agreement.SignedDate:yyyy-MM-dd}</p>");

            if (!string.IsNullOrEmpty(agreement.SignatureData))
            {
                pdfHtml.AppendLine("<div class='signature'>");
                pdfHtml.AppendLine("<h3>Signature:</h3>");
                pdfHtml.AppendLine($"<img src='{agreement.SignatureData}' style='max-width: 76%;'/>");
                pdfHtml.AppendLine("</div>");
            }

            pdfHtml.AppendLine($"<p><hr></p>");
            pdfHtml.AppendLine($"<p><strong>Doctor's Name & Surname:</strong> {agreement.drname} {agreement.drsurname}</p>");

            pdfHtml.AppendLine("</body></html>");

            return _pdfService.GeneratePdf(pdfHtml.ToString(), "AgreementConfirmation");
        }

        private async Task SaveFile(IFormFile file, string path)
        {
            using var stream = new FileStream(path, FileMode.Create);
            await file.CopyToAsync(stream);
        }





        // Controllers/AgreementController.cs
        public ActionResult ViewAllAgreements()
        {
            // Get all agreements from database
            var agreements = _context.Agreements
                .OrderByDescending(a => a.datecaputred)
                .ToList();

            // Map to ViewModel
            var viewModel = agreements.Select(a => new AgreementViewModel
            {
                Id = a.Id,
                DrName = a.drname,
                DrSurname = a.drsurname,
                DrCell = a.drcell,
                PhysEmail = a.physemail,
                DateCaptured = a.datecaputred
            }).ToList();

            return View(viewModel);
        }
    }

}