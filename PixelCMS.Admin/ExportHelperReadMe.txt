
This file explains how to use the ExcelExportHelper class. 

The export helper has three parameters (file name, sheet name, ICollection of data). You must set the type of the

export helper when making the call. For example: var file = new ExcelExportHelper<User>(fileName, sheetName, users).CreateSheet();

In this call ExcelExportHelper<YOUR TYPE> your type would refer to your model or viewmodel. This would be the same type of ICollection that you are using.

For example: var users = new HashSet<User>(); Although we are creating the collection on the fly in our example this would hold true if the 

ICollection was part of your model. (ICollection<User>). Note: the export class returns a FileStreamResult which can be downloaded as the file. 

Below is an example of the call:

        [HttpPost]
        public ActionResult Index(YOUR MODEL) 
        {
            string sheetName = "Users"; //Set the sheet name to be passed in.

            string fileName = "MyListOfUsers"; //Set the file name that will be saved.

            //Below we are building a collection of users to pass to the export helper. Notice we are 
            //using a list of type User. This is important because you must set the type for the Export helper.
            //You can skip this part as you should have an ICollection of data to pass into the helper.            

            var user1 = new User();
            user1.Id=1;
            user1.Name="Jane";

            var user2 = new User();
            user2.Id=2;
            user2.Name="John";

            var users = new HashSet<User>();
            users.Add(user1);
            users.Add(user2);
			
            //Now that we have a collection of users and we want to export this data to excel we can call the export helper.
            
            var file = new ExcelExportHelper<User>(fileName, sheetName, users).CreateSheet();
            
            //Again, we set the type of the export helper to our model of User and passed in the required parameters. The CreateSheet method 
            //returns a FileStreamResult that can be returned to the client for download.            

            return file;
        }
        
        //This is a simple data dump into excel and the package that this is created from (Epplus) has a large range of options that can be implemented.
        //If you are looking for the full range of features, I would highly recommend Epplus. Thank you for checking out my first NuGet package!        
