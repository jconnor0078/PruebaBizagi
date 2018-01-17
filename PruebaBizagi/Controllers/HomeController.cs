using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PruebaBizagi.BLL.Common;
using PruebaBizagi.BLL;
using PruebaBizagi.ViewModels;
using Newtonsoft.Json;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using PruebaBizagi.DAL;
using System.IO;

namespace PruebaBizagi.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var model = new FormViewModel();

            //buscando la data de los dropdowns
            model.fuenteIngreso = ufillFuenteIngreso();
            model.tipoVivienda = ufillTipoVivienda();
            model.tipoVehiculo = ufillTipoVehiculo();
            model.marca = ufillMarcaVehiculo();
            model.condicion = ufillCondicion();
            model.plazo = ufillPeriodo();
            model.modelo = ufillModeloVehiculo(0);
            model.periodoTasaFija = ufillTasaConfig(0);


            return View(model);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpPost]
        public JsonResult validate()
        {
            //sacando la cedula
            var cedula = System.Web.HttpContext.Current.Request.Form["HelpCedula"];

            //sacando los archivos
            var pic = System.Web.HttpContext.Current.Request.Files["HelpSectionImages"];
            HttpPostedFileBase filebase = new HttpPostedFileWrapper(pic);

            string FileStr = null;
            string fileName = "";
            if (filebase != null)
            {
                if (filebase.FileName != null)
                {
                    fileName = filebase.FileName;
                }

                FileStr = PaseToBase64(filebase);
            }
            PruebaBizagi.WorkFlowBizagiService.WorkflowEngineSOASoapClient serv = new PruebaBizagi.WorkFlowBizagiService.WorkflowEngineSOASoapClient();


            // using System.Xml;
            //creando caso
            String rawXml =
                  @"<BizAgiWSParam>
	                    <domain>domain</domain>
	                    <userName>oficial02</userName>
	                    <Cases>
		                    <Case>
			                    <Process>LoanRequest</Process>
			                    <Entities>
				                    <LoanRequest>
					
				                    </LoanRequest>
			                    </Entities>
		                    </Case>
	                    </Cases>
                    </BizAgiWSParam>";

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(rawXml);

            var resultsss = serv.createCases(xmlDoc);

            if (resultsss.SelectSingleNode("process/processId").InnerXml != null && resultsss.SelectSingleNode("process/processId").InnerXml != "")
            {
                //realizando la primera actividad.
                var caseid = resultsss.SelectSingleNode("process/processId").InnerXml;
                String rawXml2 =
                         @"<BizAgiWSParam>
                              <domain>domain</domain>
                              <userName>Oficial02</userName>
                              <ActivityData>
                                <idCase>" + caseid + @"</idCase>
                                <taskName>Registrar</taskName>
                              </ActivityData>
                              <Entities>
				                <LoanRequest>
                                <TGEN_Oficina>
						    <idTGEN_SUCURSAL>4</idTGEN_SUCURSAL>
						    <OFI_NOMBRE>SUC. FERRETERIA OCHOA</OFI_NOMBRE>
						    <OFI_CODSUC>3</OFI_CODSUC>
					    </TGEN_Oficina>
					    <TGEN_Sucursal>
						    <SUC_CODIGO>3</SUC_CODIGO>
						    <SUC_NOMBRE>FERRETERIA OCHOA</SUC_NOMBRE>
					    </TGEN_Sucursal>
					    <Promocion>1</Promocion>
					    <DocumentTypes>1</DocumentTypes>
			 
					    <DocumentNumber>224-0018986-0</DocumentNumber>
                        <LogInvokeServices>
                        </LogInvokeServices>
                        </LoanRequest>
			                </Entities>
                            </BizAgiWSParam>";

                XmlDocument xmlDoc2 = new XmlDocument();
                xmlDoc2.LoadXml(rawXml2);

                var resultsss2 = serv.performActivity(xmlDoc2);
                var strng3 = "";
                if (FileStr != null)
                {
                    strng3 = @"
                    <BizAgiWSParam>
                          <domain>domain</domain>
                          <userName>Oficial02</userName>
                          <ActivityData>
                            <idCase>" + caseid + @"</idCase>
                            <taskName>Info_ValCredito</taskName>
                          </ActivityData>
                          <Entities>
				            <LoanRequest>
                                <LoanDocuments>
                                    <IdentificationFile>
                                        <File fileName=" + '\u0022' + fileName + '\u0022' + @">" + FileStr + @"</File>
                                    </IdentificationFile>
                                </LoanDocuments>    
	                            <TasaConfig>6</TasaConfig>
	                            <CredinetInitialData>
		                            <CodigoPromotor>852336</CodigoPromotor>
		                            <VendedorBanco>Miguel Martinez</VendedorBanco>
		                            <Dealer>6</Dealer>
		                            <VendedorDealer>Jefferson Connor</VendedorDealer>
		                            <SubProducto>5</SubProducto>
		                            <TipoDocVehiculo>
			                            <idTipoDocVehiculo>1</idTipoDocVehiculo>
			                            <Descripcion>Matrícula</Descripcion>
		                            </TipoDocVehiculo>
		                            <TIPOVEHICULO>
			                            <DES_CODIGO>1</DES_CODIGO>	
			                            <DES_DESCRIPCION>VEHICULO</DES_DESCRIPCION>
			                            <DES_TUCodigo>1</DES_TUCodigo>
		                            </TIPOVEHICULO>
		                            <MODELO>94</MODELO>
                                    <MARCA>1</MARCA>
		                            <ANOFABVEHICULO>2017</ANOFABVEHICULO>
		                            <TASATOTAL_CFG>14.95</TASATOTAL_CFG>
                                    <TASA_PACTADA>14.95</TASA_PACTADA>
		                            <Frecuencia>
			                            <idFrecuencia>4</idFrecuencia>
			                            <Description>Mensual</Description>
			                            <Fre_Cod>4</Fre_Cod>
		                            </Frecuencia>
		                            <Period>1</Period>
		                            <Moneda>
			                            <idTEGEN_MONEDA>4</idTEGEN_MONEDA>
			                            <MON_DESC>PESOS DOMINICANOS</MON_DESC>
			                            <MON_ABR>RD$</MON_ABR>
			                            <MON_COD>0</MON_COD>
		                            </Moneda>
		                            <MontoSolicitado>400000</MontoSolicitado>
		                            <VALOR>600000</VALOR>
		                            <CONDICION>2</CONDICION>
		                            <TELEFONOCELULAR>809-652-8008</TELEFONOCELULAR>
		                            <FuenteIngreso>1</FuenteIngreso>
		                            <INGRESOSMENSUALES>85000</INGRESOSMENSUALES>
		                            <TIPOVIVIENDA>1</TIPOVIVIENDA>
		                            <LUGARTRABAJO>Banco StartNew</LUGARTRABAJO>
		                            <PosicionActual>Ing Analista</PosicionActual>
		                            <AnoServicio>5</AnoServicio>
	                            </CredinetInitialData>
	                            <LogInvokeServices>
	                            </LogInvokeServices>
                            </LoanRequest>
			            </Entities>
                  </BizAgiWSParam>";
                }
                else
                {
                    strng3 = @"
                    <BizAgiWSParam>
                          <domain>domain</domain>
                          <userName>Oficial02</userName>
                          <ActivityData>
                            <idCase>" + caseid + @"</idCase>
                            <taskName>Info_ValCredito</taskName>
                          </ActivityData>
                          <Entities>
				            <LoanRequest>
	                            <TasaConfig>6</TasaConfig>
	                            <CredinetInitialData>
		                            <CodigoPromotor>852336</CodigoPromotor>
		                            <VendedorBanco>Miguel Martinez</VendedorBanco>
		                            <Dealer>6</Dealer>
		                            <VendedorDealer>Jefferson Connor</VendedorDealer>
		                            <SubProducto>5</SubProducto>
		                            <TipoDocVehiculo>
			                            <idTipoDocVehiculo>1</idTipoDocVehiculo>
			                            <Descripcion>Matrícula</Descripcion>
		                            </TipoDocVehiculo>
		                            <TIPOVEHICULO>
			                            <DES_CODIGO>1</DES_CODIGO>	
			                            <DES_DESCRIPCION>VEHICULO</DES_DESCRIPCION>
			                            <DES_TUCodigo>1</DES_TUCodigo>
		                            </TIPOVEHICULO>
		                            <MODELO>94</MODELO>
                                    <MARCA>1</MARCA>
		                            <ANOFABVEHICULO>2017</ANOFABVEHICULO>
		                            <TASATOTAL_CFG>14.95</TASATOTAL_CFG>
                                    <TASA_PACTADA>14.95</TASA_PACTADA>
		                            <Frecuencia>
			                            <idFrecuencia>4</idFrecuencia>
			                            <Description>Mensual</Description>
			                            <Fre_Cod>4</Fre_Cod>
		                            </Frecuencia>
		                            <Period>1</Period>
		                            <Moneda>
			                            <idTEGEN_MONEDA>4</idTEGEN_MONEDA>
			                            <MON_DESC>PESOS DOMINICANOS</MON_DESC>
			                            <MON_ABR>RD$</MON_ABR>
			                            <MON_COD>0</MON_COD>
		                            </Moneda>
		                            <MontoSolicitado>400000</MontoSolicitado>
		                            <VALOR>600000</VALOR>
		                            <CONDICION>2</CONDICION>
		                            <TELEFONOCELULAR>809-652-8008</TELEFONOCELULAR>
		                            <FuenteIngreso>1</FuenteIngreso>
		                            <INGRESOSMENSUALES>85000</INGRESOSMENSUALES>
		                            <TIPOVIVIENDA>1</TIPOVIVIENDA>
		                            <LUGARTRABAJO>Banco StartNew</LUGARTRABAJO>
		                            <PosicionActual>Ing Analista</PosicionActual>
		                            <AnoServicio>5</AnoServicio>
	                            </CredinetInitialData>
	                            <LogInvokeServices>
	                            </LogInvokeServices>
                            </LoanRequest>
			            </Entities>
                  </BizAgiWSParam>";

                }

                XmlDocument final = new XmlDocument();
                final.LoadXml(strng3);

                var tmpResult = serv.performActivity(final);
            }


            var result = new PadronResponse();

            var repo = new Repository();
            result = repo.getPadron(cedula);

            byte[] newBytes = Convert.FromBase64String(result.foto);

            result.fotobyte = newBytes;

            return Json(result);

        }

        [HttpPost]
        public ActionResult sumit(FormViewModel model)
        {
            var result = enviarToBizagi(model);

            //buscando la data de los dropdowns
            model.fuenteIngreso = ufillFuenteIngreso();
            model.tipoVivienda = ufillTipoVivienda();
            model.tipoVehiculo = ufillTipoVehiculo();
            model.marca = ufillMarcaVehiculo();
            model.condicion = ufillCondicion();
            model.plazo = ufillPeriodo();
            model.modelo = ufillModeloVehiculo(model.marcaId > 0 ? model.marcaId : 0);
            model.periodoTasaFija = ufillTasaConfig(model.anoVehiculo > 0 ? model.anoVehiculo : 0);
            model.isOkRequest = result.isOk?"true":"false";
            model.msjRequest = result.mjs;

            return View("Index", model);

        }

        #region utilidades
        [HttpPost]
        public JsonResult ugetModels()
        {
            //sacando el id de la marca
            string idstr = System.Web.HttpContext.Current.Request.Form["IDMARCA"];
            int id = 0;
            Int32.TryParse(idstr, out id);
            var result = ufillModeloVehiculo(id);
            return Json(result);
        }

        [HttpPost]
        public JsonResult ugetTasaConfig()
        {
            //sacando el id de la marca
            string idstr = System.Web.HttpContext.Current.Request.Form["ANOVEH"];
            int id = 0;
            Int32.TryParse(idstr, out id);
            var result = ufillTasaConfig(id);
            return Json(result);
        }


        public static string Serialize<T>(T dataToSerialize)
        {
            try
            {
                var stringwriter = new System.IO.StringWriter();
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(stringwriter, dataToSerialize);
                return stringwriter.ToString();
            }
            catch
            {
                throw;
            }
        }

        public static string PaseToBase64(HttpPostedFileBase cvFile)
        {
            int fileSizeInBytes = cvFile.ContentLength;
            MemoryStream target = new MemoryStream();
            cvFile.InputStream.CopyTo(target);
            byte[] data = target.ToArray();

            String s = Convert.ToBase64String(data);
            return s;
        }
        #endregion

        #region metodos de los dropdowns
        public List<SelectListItem> ufillFuenteIngreso()
        {
            var tmpResult = new List<SelectListItem>();



            using (var tmpContext = new LoanReq_FeriaDigitalEntities())
            {
                var ListObj = tmpContext.FuenteIngresoes.ToList();
                if (ListObj != null && ListObj.Count > 0)
                {
                    var tmpList = ListObj.Select(c => new SelectListItem()
                    {
                        Text = c.Description,
                        Value = c.idFuenteIngreso.ToString()
                    }).ToList();

                    if (tmpList != null && tmpList.Count > 0)
                    {
                        tmpResult.AddRange(tmpList);
                    }
                }
            }
            return tmpResult;
        }

        public List<SelectListItem> ufillTipoVivienda()
        {
            var tmpResult = new List<SelectListItem>();



            using (var tmpContext = new LoanReq_FeriaDigitalEntities())
            {
                var ListObj = tmpContext.TGEN_TIPOVIVIENDA.ToList();
                if (ListObj != null && ListObj.Count > 0)
                {
                    var tmpList = ListObj.Select(c => new SelectListItem()
                    {
                        Text = c.DESCRIPTION,
                        Value = c.idTGEN_TIPOVIVIENDA.ToString()
                    }).ToList();

                    if (tmpList != null && tmpList.Count > 0)
                    {
                        tmpResult.AddRange(tmpList);
                    }
                }
            }
            return tmpResult;
        }

        public List<SelectListItem> ufillTipoVehiculo()
        {
            var tmpResult = new List<SelectListItem>();



            using (var tmpContext = new LoanReq_FeriaDigitalEntities())
            {
                var ListObj = tmpContext.TGEN_DESCTABLA7.ToList();
                if (ListObj != null && ListObj.Count > 0)
                {
                    var tmpList = ListObj.Select(c => new SelectListItem()
                    {
                        Text = c.DES_DESCRIPCION,
                        Value = c.DES_CODIGO.ToString(),
                    }).ToList();

                    if (tmpList != null && tmpList.Count > 0)
                    {
                        tmpResult.AddRange(tmpList);
                    }
                }
            }
            return tmpResult;
        }

        public List<SelectListItem> ufillMarcaVehiculo()
        {
            var tmpResult = new List<SelectListItem>();



            using (var tmpContext = new LoanReq_FeriaDigitalEntities())
            {
                var ListObj = tmpContext.TGEN_MARCA.ToList();
                if (ListObj != null && ListObj.Count > 0)
                {
                    var tmpList = ListObj.Select(c => new SelectListItem()
                    {
                        Text = c.DESCRIPTION,
                        Value = c.SECUENCE.ToString(),
                    }).ToList();

                    if (tmpList != null && tmpList.Count > 0)
                    {
                        tmpResult.AddRange(tmpList);
                    }
                }
            }
            return tmpResult;
        }

        public List<SelectListItem> ufillModeloVehiculo(long id)
        {
            var tmpResult = new List<SelectListItem>();


            if (id > 0)
            {
                using (var tmpContext = new LoanReq_FeriaDigitalEntities())
                {
                    var ListObj = tmpContext.TGEN_MODELO.Where(w => w.MARCA == id).ToList();
                    if (ListObj != null && ListObj.Count > 0)
                    {
                        var tmpList = ListObj.Select(c => new SelectListItem()
                        {
                            Text = c.DESCRIPTION,
                            Value = c.idTGEN_MODELO.ToString(),
                        }).ToList();

                        if (tmpList != null && tmpList.Count > 0)
                        {
                            tmpResult.AddRange(tmpList);
                        }
                    }
                }
            }
            return tmpResult;
        }

        public List<SelectListItem> ufillCondicion()
        {
            var tmpResult = new List<SelectListItem>();



            using (var tmpContext = new LoanReq_FeriaDigitalEntities())
            {
                var ListObj = tmpContext.TGEN_CONDICION.ToList();
                if (ListObj != null && ListObj.Count > 0)
                {
                    var tmpList = ListObj.Select(c => new SelectListItem()
                    {
                        Text = c.DESCRIPTION,
                        Value = c.idTGEN_CONDICION.ToString(),
                    }).ToList();

                    if (tmpList != null && tmpList.Count > 0)
                    {
                        tmpResult.AddRange(tmpList);
                    }
                }
            }
            return tmpResult;
        }

        public List<SelectListItem> ufillTasaConfig(int ano)
        {
            var tmpResult = new List<SelectListItem>();


            if (ano > 0)
            {
                using (var tmpContext = new LoanReq_FeriaDigitalEntities())
                {
                    var ListObj = tmpContext.TasasConfigs.Where(w => w.FromVehicleYear <= ano && w.UntilVehicleYear >= ano).ToList();
                    if (ListObj != null && ListObj.Count > 0)
                    {
                        var tmpList = ListObj.Select(c => new SelectListItem()
                        {
                            Text = c.Description,
                            Value = c.idTasasConfig.ToString(),
                        }).ToList();

                        if (tmpList != null && tmpList.Count > 0)
                        {
                            tmpResult.AddRange(tmpList);
                        }
                    }
                }
            }
            return tmpResult;
        }

        public List<SelectListItem> ufillPeriodo()
        {
            var tmpResult = new List<SelectListItem>();



            using (var tmpContext = new LoanReq_FeriaDigitalEntities())
            {
                var ListObj = tmpContext.Periods.ToList();
                if (ListObj != null && ListObj.Count > 0)
                {
                    var tmpList = ListObj.Select(c => new SelectListItem()
                    {
                        Text = c.Description,
                        Value = c.PeriodId.ToString(),
                    }).ToList();

                    if (tmpList != null && tmpList.Count > 0)
                    {
                        tmpResult.AddRange(tmpList);
                    }
                }
            }
            return tmpResult;
        }
        #endregion


        #region Codigo del formulario

        public responseHelper enviarToBizagi(FormViewModel model)
        {
            var result = new responseHelper();
            string CaseId = "";
            try
            {
                result = Bizagi_CrearCaso(model, out CaseId);
                if (result.isOk)
                {
                    result = Bizagi_EjecutarActRegistrar(model, CaseId);
                    if (result.isOk)
                    {
                        result =Bizagi_EjecutarActInfo_ValCredito(model, CaseId);
                    }
                }
            }
            catch (Exception ex)
            {
                result.setError("Ha ocurrido un error " + ex.Message);
            }

            return result;
        }

        //Metodo para crear un caso en bizagi
        public responseHelper Bizagi_CrearCaso(FormViewModel model, out string CaseId)
        {
            var result = new responseHelper();
            CaseId = "";
            try
            {
                PruebaBizagi.WorkFlowBizagiService.WorkflowEngineSOASoapClient serv = new PruebaBizagi.WorkFlowBizagiService.WorkflowEngineSOASoapClient();
                String rawXml =
                    @"<BizAgiWSParam>
	                    <domain>domain</domain>
	                    <userName>oficial02</userName>
	                    <Cases>
		                    <Case>
			                    <Process>LoanRequest</Process>
			                    <Entities>
				                    <LoanRequest>
					
				                    </LoanRequest>
			                    </Entities>
		                    </Case>
	                    </Cases>
                    </BizAgiWSParam>";
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(rawXml);

                var webServiceResult = serv.createCases(xmlDoc);

                if (webServiceResult.SelectSingleNode("process/processId").InnerXml != null
                    && webServiceResult.SelectSingleNode("process/processId").InnerXml != ""
                    && webServiceResult.SelectSingleNode("process/processId").InnerXml != "0")
                {
                    var caseid = webServiceResult.SelectSingleNode("process/processId").InnerXml;
                    if (caseid != null && caseid != string.Empty)
                    {
                        CaseId = caseid;
                        result.setOk();
                    }
                    else
                    {
                        result.setError("No se pudo crear la solicitud, porque no se creó el caso.");
                    }
                }
                else
                {
                    result.setError("No se pudo crear la solicitud, porque no se creó el caso.");
                }
            }
            catch (Exception ex)
            {
                result.setError("Ha ocurrido un error " + ex.Message); ;
            }
            return result;
        }

        public responseHelper Bizagi_EjecutarActRegistrar(FormViewModel model, string CaseId)
        {
            var result = new responseHelper();
            try
            {
                PruebaBizagi.WorkFlowBizagiService.WorkflowEngineSOASoapClient serv = new PruebaBizagi.WorkFlowBizagiService.WorkflowEngineSOASoapClient();
                String rawXml =
                    @"<BizAgiWSParam>
                          <domain>domain</domain>
                          <userName>Oficial02</userName>
                          <ActivityData>
                            <idCase>" + CaseId + @"</idCase>
                            <taskName>Registrar</taskName>
                          </ActivityData>
                          <Entities>
				           <LoanRequest>
                            <TGEN_Oficina>
						<idTGEN_SUCURSAL>4</idTGEN_SUCURSAL>
						<OFI_NOMBRE>SUC. FERRETERIA OCHOA</OFI_NOMBRE>
						<OFI_CODSUC>3</OFI_CODSUC>
					</TGEN_Oficina>
					<TGEN_Sucursal>
						<SUC_CODIGO>3</SUC_CODIGO>
						<SUC_NOMBRE>FERRETERIA OCHOA</SUC_NOMBRE>
					</TGEN_Sucursal>
					<Promocion>1</Promocion>
					<DocumentTypes>1</DocumentTypes>
			 
					<DocumentNumber>" + model.cedula + @"</DocumentNumber>
                    <LogInvokeServices>
                    </LogInvokeServices>
                    </LoanRequest>
			            </Entities>
                        </BizAgiWSParam>";
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(rawXml);

                var webServiceResult = serv.performActivity(xmlDoc);

                if (webServiceResult.SelectSingleNode("process/processId").InnerXml != null
                    && webServiceResult.SelectSingleNode("process/processId").InnerXml != ""
                    && webServiceResult.SelectSingleNode("process/processId").InnerXml != "0")
                {
                    result.setOk();
                }
                else
                {
                    result.setError("No se pudo completar la actividad Registrar, porque "+ webServiceResult.SelectSingleNode("process/processError/errorMessage").InnerXml);
                }
            }
            catch (Exception ex)
            {
                result.setError("Ha ocurrido un error completando la actividad Registrar: " + ex.Message); ;
            }
            return result;
        }

        public responseHelper Bizagi_EjecutarActInfo_ValCredito(FormViewModel model, string CaseId)
        {
            var result = new responseHelper();
            try
            {
                //buscando la tasa segun el id de tasaconfig
                double tasa = 0.00D;
                using (var tmpcontext= new PruebaBizagi.DAL.LoanReq_FeriaDigitalEntities())
                {
                    var tmpobj = tmpcontext.TasasConfigs.Where(w=>w.idTasasConfig== model.periodoTasaFijaId).FirstOrDefault();
                    if (tmpobj==null)
                    {
                        result.setError("No se pudo encontrar la tasa configurada.");
                        return result;
                    }
                    tasa = tmpobj.Tasa != null ? tmpobj.Tasa.Value : 0.00D;
                    if (tasa <= 0)
                    {
                        result.setError("No se pudo encontrar la tasa configurada.");
                        return result;
                    }
                }
                
                PruebaBizagi.WorkFlowBizagiService.WorkflowEngineSOASoapClient serv = new PruebaBizagi.WorkFlowBizagiService.WorkflowEngineSOASoapClient();
                String rawXml =
                    @"
                    <BizAgiWSParam>
                          <domain>domain</domain>
                          <userName>Oficial02</userName>
                          <ActivityData>
                            <idCase>" + CaseId + @"</idCase>
                            <taskName>Info_ValCredito</taskName>
                          </ActivityData>
                          <Entities>
				            <LoanRequest>
	                            <TasaConfig>" + model.periodoTasaFijaId+ @"</TasaConfig>
	                            <CredinetInitialData>
		                            <CodigoPromotor>852336</CodigoPromotor>
		                            <VendedorBanco>Miguel Martinez</VendedorBanco>
		                            <Dealer>6</Dealer>
		                            <VendedorDealer>Jefferson Connor</VendedorDealer>
		                            <SubProducto>5</SubProducto>
		                            <TipoDocVehiculo>
			                            <idTipoDocVehiculo>1</idTipoDocVehiculo>
			                            <Descripcion>Matrícula</Descripcion>
		                            </TipoDocVehiculo>
		                            <TIPOVEHICULO>" + model.tipoVehiculoId + @"</TIPOVEHICULO>
		                            <MODELO>" + model.modeloId + @"</MODELO>
                                    <MARCA>" + model.marcaId + @"</MARCA>
		                            <ANOFABVEHICULO>" + model.anoVehiculo + @"</ANOFABVEHICULO>
		                            <TASATOTAL_CFG>" + tasa + @"</TASATOTAL_CFG>
                                    <TASA_PACTADA>" + tasa + @"</TASA_PACTADA>
		                            <Frecuencia>
			                            <idFrecuencia>4</idFrecuencia>
			                            <Description>Mensual</Description>
			                            <Fre_Cod>4</Fre_Cod>
		                            </Frecuencia>
		                            <Period>" + model.plazoId + @"</Period>
		                            <Moneda>
			                            <idTEGEN_MONEDA>4</idTEGEN_MONEDA>
			                            <MON_DESC>PESOS DOMINICANOS</MON_DESC>
			                            <MON_ABR>RD$</MON_ABR>
			                            <MON_COD>0</MON_COD>
		                            </Moneda>
		                            <MontoSolicitado>" + model.montoSolicitado + @"</MontoSolicitado>
		                            <VALOR>" + model.valorVehiculo + @"</VALOR>
		                            <CONDICION>" + model.condicionId + @"</CONDICION>
		                            <TELEFONOCELULAR>" + model.telefonoCelular + @"</TELEFONOCELULAR>
		                            <FuenteIngreso>" + model.fuenteIngresoId + @"</FuenteIngreso>
		                            <INGRESOSMENSUALES>" + model.ingresosMensuales + @"</INGRESOSMENSUALES>
		                            <TIPOVIVIENDA>" + model.tipoViviendaId + @"</TIPOVIVIENDA>
		                            <LUGARTRABAJO>" + model.lugarTrabajo + @"</LUGARTRABAJO>
		                            <PosicionActual>" + model.posicionActual + @"</PosicionActual>
		                            <AnoServicio>" + model.anoServicios + @"</AnoServicio>
	                            </CredinetInitialData>
	                            <LogInvokeServices>
	                            </LogInvokeServices>
                            </LoanRequest>
			            </Entities>
                  </BizAgiWSParam>"; ;
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(rawXml);

                var webServiceResult = serv.performActivity(xmlDoc);

                if (webServiceResult.SelectSingleNode("process/processId").InnerXml != null
                    && webServiceResult.SelectSingleNode("process/processId").InnerXml != ""
                    && webServiceResult.SelectSingleNode("process/processId").InnerXml != "0")
                {
                    result.setOk();
                }
                else
                {
                    result.setError("No se pudo completar la actividad Registrar, porque " + webServiceResult.SelectSingleNode("process/processError/errorMessage").InnerXml);
                }
            }
            catch (Exception ex)
            {
                result.setError("Ha ocurrido un error completando la actividad Registrar: " + ex.Message); ;
            }
            //Bizagi_AbortarCaso(CaseId);
            return result;
        }

        public void Bizagi_AbortarCaso(string CaseId)
        {
            var result = new responseHelper();
            try
            {
                PruebaBizagi.WorkFlowBizagiService.WorkflowEngineSOASoapClient serv = new PruebaBizagi.WorkFlowBizagiService.WorkflowEngineSOASoapClient();
                String rawXml =
                    @"<BizAgiWSParam>
                         <domain>domain</domain>
                         <userName>oficial02</userName>
                         <cases>
                                 <case>
                                         <idCase>" + CaseId + @"</idCase>
                                         <abortReason>Invalid case</abortReason>
                                 </case>              
                         </cases>
                    </BizAgiWSParam>";
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(rawXml);
                var webServiceResult = serv.abortCases(xmlDoc);
            }
            catch (Exception ex)
            {
                result.setError("Ha ocurrido un error " + ex.Message); ;
            }
        }

        #endregion




    }

    #region clase helper
    public class responseHelper
    {
        public bool isOk { get; set; }
        public string mjs { get; set; }

        public void setError(string vmjs)
        {
            isOk = false;
            mjs = vmjs;
        }

        public void setOk()
        {
            isOk = true;
            mjs = "";
        }
    }
    #endregion


}