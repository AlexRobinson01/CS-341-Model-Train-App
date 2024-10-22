namespace ModelTrain.Model;
public class BusinessLogic : IBusinessLogic
    {
        private IDatabase Database { get; set; }

        public BusinessLogic()
        {
            Database = new Database(); 
        }
    }
