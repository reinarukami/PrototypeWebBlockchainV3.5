using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Npgsql;
using PrototypeWebBlockchain.Repository;
using PrototypeWebBlockchain.Models;
using System.IO;
using System.Security.Cryptography;
using System.Configuration;
using Newtonsoft.Json;
using PrototypeWebBlockchain.Functions.Filters;
using System.Diagnostics;

namespace PrototypeWebBlockchain.Controllers
{
    [AuthorizeUser]
    public class TransactionController : Controller
    {
        private readonly TransactionRepository transactionRepository;
        private readonly FileUpload fileupload;
       
        
        public TransactionController()
        {
            transactionRepository = new TransactionRepository();
            fileupload = new FileUpload();
        }

        // GET: Home

        public ActionResult UploadFile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFile(FileClass _file)
        {
            if (HttpContext.Request.Files[0].ContentLength == 0)
            {
                ModelState.AddModelError("file", "File is null , Please set Image");
                return View();
            }

            _file.image = HttpContext.Request.Files[0];


            if (_file.image != null)
            {
                fileupload.UploadFile(_file, transactionRepository, Session["ID"].ToString());
            }

            return null;
        }


        public ActionResult Transactionlist()
        {

            return View();
        }

        [HttpPost]
        public JsonResult ValidateImages(string data)
        {
            var _imageFiles = JsonConvert.DeserializeObject<List<ImageJson>>(data);
            var _imageSaved = transactionRepository.FindByID(Int32.Parse(Session["ID"].ToString()));
            var transaction = new List<TransactionJson>();

            var nfilepath = ConfigurationManager.AppSettings["FileImagePath"];

            string file;

            foreach (var item in _imageFiles)
            {

                var image = _imageSaved.Where(r => r.id == item.id).FirstOrDefault();

                try
                {
                    file = Path.Combine(nfilepath, image.filename);
                    string shavalue = fileupload.ConvertSavedFileToSha(file);
                    if (shavalue == item.filehash)
                    {
                        item.filehash = image.filename;
                        transaction.Add(new TransactionJson()
                        {
                            id = item.id,
                            filename = image.filename,
                            status = "Status/check.png",
                            date = item.date
                        });
                    }
                    else
                    {
                        transaction.Add(new TransactionJson()
                        {
                            id = item.id,
                            filename = image.filename,
                            status = "Status/cross.png",
                            date = item.date
                        });

                    }
                }

                catch
                {
                    transaction.Add(new TransactionJson()
                    {
                        id = item.id,
                        filename = image.filename,
                        status = "Status/cross.png",
                        date = item.date
                    });
                }
            }

            return new JsonResult() { Data = new { JTransaction = transaction } };
        }

    }
}