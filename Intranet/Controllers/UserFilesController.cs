using Intranet.Models;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace Intranet.Controllers
{
    [Authorize]
    public class UserFilesController : Controller
    {
        PubSystemEntities data = new PubSystemEntities();

        [HttpPost, ValidateInput(false)]
        public ActionResult Upload1(HttpPostedFileWrapper[] files, int id_folder = 0)
        {
            List<FileViewModel> listFiles = new List<FileViewModel>();

            foreach (var f in files)
            {
                var dbfile = SaveUpload(f, ref id_folder);

                listFiles.Add(new FileViewModel
                {
                    CanDelete = false,
                    Id = dbfile.id_file,
                    Folder = false,
                    Image = dbfile.image,
                    Name = dbfile.name,
                    Width = dbfile.width,
                    Height = dbfile.height,
                    Size = dbfile.filesize,
                    URL = GetFilePath(dbfile),
                    Thumbnail = getThumbNailPath(dbfile),
                    UploadDate = dbfile.upload_date,
                    NewItem = true

                });

            }

            int[] newItems = (int[])(Session["NewItems"]) ?? new int[0];

            Session["NewItems"] =  listFiles.Select(f => f.Id).Union(newItems).ToArray();

            //return Json(new { id_folder = id_folder });
            return PartialView("_FileList", listFiles);
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult Upload(HttpPostedFileWrapper upload = null)
        {
            int id_folder = 0;
            var dbfile = SaveUpload(upload, ref id_folder);
            ViewData["FileUrl"] = GetFilePath(dbfile); 
            return View();
        }


        private Intranet.File SaveUpload(HttpPostedFileWrapper upload, ref int id_folder)
        {
            string uploadPath = Intranet.Properties.Settings.Default.UploadPath;
            Intranet.File dbfile = null;

            if (upload != null)
            {
                int id_user = Int32.Parse(User.Identity.GetUserId());


                Folder folder;

                if (id_folder == 0)
                {
                    folder = GetUserRootFolder(id_user);
                    id_folder = folder.id_folder;
                }
                else
                {
                    int tmp_id_folder = id_folder;
                    folder = data.Folders.SingleOrDefault(f => f.id_folder == tmp_id_folder && f.id_user == id_user);
                }

                data.Database.Connection.Open();

                var rnd = new Random();
                int newid = rnd.Next(1000000000);
                
                dbfile = new Intranet.File
                {
                    id_file = newid,
                    id_user = id_user,
                    id_folder = folder.id_folder,
                    filename = "?",
                    fileext = Path.GetExtension(upload.FileName),
                    filesize = 0,
                    height = 0,
                    width = 0,
                    image = false,
                    name = upload.FileName,
                    upload_date = DateTime.Now
                };

                while (data.Files.Any(f => f.id_file == newid))
                {
                    newid = rnd.Next(1000000000);
                }

                data.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Files] ON");
                data.Files.Add(dbfile);
                data.SaveChanges();
                data.Database.ExecuteSqlCommand("SET IDENTITY_INSERT [dbo].[Files] OFF");

                string path = System.IO.Path.Combine(uploadPath, dbfile.id_file.ToString() + dbfile.fileext);

                upload.SaveAs(path);

                var finfo = new System.IO.FileInfo(path);

                dbfile.filesize = (int)finfo.Length;
                dbfile.filename = dbfile.id_file.ToString() + dbfile.fileext;

                try
                {
                    Image image = Image.FromFile(path);

                    dbfile.image = true;
                    dbfile.width = image.Width;
                    dbfile.height = image.Height;

                    image.Dispose();
                }
                catch { }

                data.SaveChanges();
            }

            return dbfile;
        }
        
        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult Browse(int id_folder = 0, int sort = 1, int filter = 0, string search = "")
        {
            if (!Request.IsAjaxRequest())
            {
                if (Session["browse_id_folder"] != null)
                {
                    id_folder = (int)(Session["browse_id_folder"]);
                }
                ViewData["id_folder"] = id_folder;
            }
            else
            {
                Session["browse_id_folder"] = id_folder;
            }            

            if (id_folder != 0)
            {
                List<Folder> path = new List<Folder>();
                int id_folder_parent = id_folder;
                while (id_folder_parent != 0)
                {
                    var fp = data.Folders.SingleOrDefault(f => f.id_folder == id_folder_parent);
                    path.Add(fp);
                    id_folder_parent = fp.id_folder_parent;
                }

                path.Reverse();
                ViewData["path"] = path.ToArray();
            }
            else
            {
                ViewData["path"] = new Folder[0];
            }
                       

            int id_user = Int32.Parse(User.Identity.GetUserId());

            int[] newItems = (int[])(Session["NewItems"]) ?? new int[0];
            int[] clipboardItems = (int[])Session["ClipboardItems"] ?? new int[0];

            int id_folder_files = id_folder;
            int id_folder_root = GetUserRootFolder(id_user).id_folder;

            if (id_folder_files == 0)
            {
                id_folder_files = id_folder_root;
            }

            var files = data
                .Files
                .Where(f => f.id_user == id_user && f.id_folder== id_folder_files && (filter == 0 || (filter == 1 && f.image) || (filter == 2 && !f.image)))
                .AsEnumerable()
                .Select(f => new FileViewModel
                {
                    Id = f.id_file,
                    URL = GetFilePath(f),
                    Thumbnail = getThumbNailPath(f),
                    Name = f.name,
                    Image = f.image,
                    Folder = false,
                    UploadDate = f.upload_date,
                    Size = f.filesize,
                    Width = f.width,
                    Height = f.height,
                    CanDelete = !data.FormItemFields.Any(fif => fif.datetype == 9 && fif.intvalue == f.id_file) ,
                    NewItem = newItems.Contains(f.id_file),
                    ClipboardItem = clipboardItems.Contains(f.id_file)
                })
                //.Where(f => search == "" || f.Name.StartsWith(search, StringComparison.CurrentCultureIgnoreCase))
                .Where(f =>
                {
                    if (search == "")
                    {
                        return true;
                    }

                    bool res = false;
                    f.Name = Regex.Replace(f.Name, search, m => { res = true; return String.Format("<mark>{0}</mark>", m.ToString()); }, RegexOptions.IgnoreCase);

                    return res;
                })
                .OrderBy(f => sort == 0 ? f.Name : "")
                .OrderByDescending(f => sort == 1 ? f.UploadDate : DateTime.Today)
                .OrderByDescending(f => sort == 2 ? f.Size: 0);

            var folders = data.Folders
                .Where(f => f.id_user == id_user && f.id_folder_parent == id_folder && f.id_folder != id_folder_root)
                .OrderBy(f => f.name)
                .AsEnumerable()
                .Select(f => new FileViewModel
                {
                    Id = f.id_folder,
                    URL = f.id_folder.ToString(),
                    Thumbnail = "",
                    Name = f.name,
                    Image = false,
                    Folder = true,
                    UploadDate = f.create_date,
                    Size = 0,
                    Width = 0,
                    Height = 0,
                    CanDelete = (f.Files.Count == 0 && !data.Folders.Any(ff => ff.id_folder_parent == f.id_folder)),
                    NewItem = newItems.Contains(-f.id_folder),
                    ClipboardItem = clipboardItems.Contains(-f.id_folder)
                })
                .Where(f =>
                {
                    if (search == "")
                    {
                        return true;
                    }

                    bool res = false;
                    f.Name = Regex.Replace(f.Name, search, m => { res = true; return String.Format("<mark>{0}</mark>", m.ToString()); }, RegexOptions.IgnoreCase);

                    return res;
                })
                .OrderBy(f => sort == 0 ? f.Name : "")
                .OrderByDescending(f => sort == 1 ? f.UploadDate : DateTime.Today)
                .ToArray();

            Session["NewItems"] = null;

            if (id_folder != 0)
            {
                var folder = data.Folders.SingleOrDefault(f => f.id_folder == id_folder);

                folders = new FileViewModel[] { new FileViewModel
                {
                    Id = folder.id_folder_parent,
                    URL = id_folder.ToString(),
                    Thumbnail = "",
                    Name = "...",
                    Image = false,
                    Folder = true,
                    UploadDate = folder.create_date,
                    Size = 0,
                    Width = 0,
                    Height = 0,
                    CanDelete = false
                }}.Union(folders).ToArray();
            }

            if (Request.IsAjaxRequest())
            {
                return PartialView("_FileList", folders.Union(files));
            }
            else
            {
                return View(folders.Union(files));
            }
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Delete(string list)
        {
            string uploadPath = Intranet.Properties.Settings.Default.UploadPath;

            foreach (string sid in list.Split(','))
            {
                int id = Int32.Parse(sid);

                if (id > 0)
                {
                    var file = data.Files.SingleOrDefault(f => f.id_file == id);
                    string path = System.IO.Path.Combine(uploadPath, file.filename);
                    try
                    {
                        System.IO.File.Delete(path);
                    }
                    catch { }
                    data.Files.Remove(file);
                    data.SaveChanges();
                }
                else
                {
                    var folder = data.Folders.SingleOrDefault(f => f.id_folder == -id);

                    if (folder.Files.Count == 0 && !data.Folders.Any(ff => ff.id_folder_parent == folder.id_folder))
                    {
                        data.Folders.Remove(folder);
                        data.SaveChanges();
                    }
                }
            }

            return Json(new {  });
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Cut(string list)
        {
            Session["ClipboardItems"] = list.Split(',').Select(s => Int32.Parse(s)).ToArray();

            return Json(new { });
        }

        [HttpPost, ValidateInput(false)]
        public ActionResult Paste(int id_folder)
        {
            int id_folder_files = id_folder;

            if (id_folder_files == 0)
            {
                id_folder_files = GetUserRootFolder(Int32.Parse(User.Identity.GetUserId())).id_folder;
            }


            int[] clipboardItems = (int[])Session["ClipboardItems"] ?? new int[0];
            bool filesToRoot = false;

            foreach (int id in clipboardItems)
            {
                if (id > 0 /*&& id_folder != 0*/)
                {
                    var file = data.Files.SingleOrDefault(f => f.id_file == id);
                    file.id_folder = id_folder_files;
                    data.SaveChanges();
                }
                //else if (id > 0 && id_folder == 0)
                //{
                //    filesToRoot = true;
                //}
                else
                {
                    var folder = data.Folders.SingleOrDefault(f => f.id_folder == -id);
                    folder.id_folder_parent = id_folder;
                    data.SaveChanges();
                }
            }


            Session["NewItems"] = Session["ClipboardItems"];

            Session["ClipboardItems"] = null;

            return Json(new { filesToRoot = filesToRoot });
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult NewFolder(string name, int id_folder_parent)
        {
            if (String.IsNullOrEmpty(name))
            {
                return Json(new { });
            }


            var newFolder = new Folder
            {
                create_date = DateTime.Now,
                id_folder_parent = id_folder_parent,
                id_user = Int32.Parse(User.Identity.GetUserId()),
                name = name,
                @public = false,
                public_write = false
            };

            data.Folders.Add(newFolder);

            data.SaveChanges();

            Session["NewItems"] = new int[] {-newFolder.id_folder};

            return Json(new { });
        }


        [HttpPost, ValidateInput(false)]
        public ActionResult Rename(string name, int id)
        {
            if (String.IsNullOrEmpty(name))
            {
                return Json(new { });
            }

            if (id < 0)
            {
                data.Folders.SingleOrDefault(f => f.id_folder == -id).name = name;
            }
            else
            {
                data.Files.SingleOrDefault(f => f.id_file == id).name = name;
            }

            data.SaveChanges();

            return Json(new { });
        }

        protected string getThumbNailPath(File file)
        {
            if (file.image)
            {
                return String.Format("{0}/getthumbnail.aspx?id_file={1}&width=128&height=128&q=90", Intranet.Properties.Settings.Default.BaseURL, file.id_file);
            }
            else if (file.fileext.ToLower() == ".pdf")
            {
                return "/Content/Img/Adobe-PDF-Document-icon.png";
            }
            else if (file.fileext.ToLower() == ".zip")
            {
                return "/Content/Img/folder-zip-icon.png";
            }
            else if (file.fileext.ToLower() == ".html" || file.fileext.ToLower() == ".htm")
            {
                return "/Content/Img/File-Adobe-Dreamweaver-HTML-01-icon.png";
            }
            else if (file.fileext.ToLower() == ".doc" || file.fileext.ToLower() == ".docx")
            {
                return "/Content/Img/Document-Microsoft-Word-icon.png";
            }
            else if (file.fileext.ToLower() == ".xls" || file.fileext.ToLower() == ".xlsx")
            {
                return "/Content/Img/Document-Microsoft-Excel-icon.png";
            }
            else if (file.fileext.ToLower() == ".ppt" || file.fileext.ToLower() == ".pptx" || file.fileext.ToLower() == ".pps")
            {
                return "/Content/Img/Document-Microsoft-PowerPoint-icon.png";
            }
            else
            {
                return "/Content/Img/Document-icon.png";
            }
        }


        protected Folder GetUserRootFolder(int id_user)
        {
            Folder folder = data.Folders.Where(f => f.id_user == id_user && f.name == "!soubory zařazené automaticky!").SingleOrDefault();

            if (folder == null)
            {
                folder = new Folder
                {
                    create_date = DateTime.Now,
                    id_folder_parent = 0,
                    id_user = id_user,
                    name = "!soubory zařazené automaticky!"
                };
                data.Folders.Add(folder);
                data.SaveChanges();
            }

            return folder;
        }

        protected string GetFilePath(File file)
        {
            return (file.image || file.fileext.ToLower().Contains(".htm")   ? "/getfile.aspx?id_file=" : "/download.aspx?id_file=") + file.id_file;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                data.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}