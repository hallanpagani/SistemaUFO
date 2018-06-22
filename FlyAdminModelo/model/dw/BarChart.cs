using System.Collections.Generic;

namespace BaseModelo.model.dw
{
    public class BarChart
    {
        public List<string> labels { get; set; }
        public List<Dataset> datasets { get; set; }

        public BarChart()
        {
            labels = new List<string>();
            datasets = new List<Dataset>();
        }
    }

    public class Dataset
    {
        public string label { get; set; }
        public string fillColor { get; set; }
        public string strokeColor { get; set; }
      //  public string highlightFill { get; set; }
        public string highlightStroke { get; set; }
        public List<double> data { get; set; }
    }

}
