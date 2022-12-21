using System;
using System.Windows.Forms;

namespace Configurator_2._0
{
    public partial class GetQuoteForm : Form
    {
        public GetQuoteForm()
        {
            InitializeComponent();
        }

        private void submit_Click(object sender, EventArgs e)
        {
            new IQuote().GetData(textBox1.Text);
        }
    }
}