using Api;
using System.Threading;

Helper apiHelper = Helper.Instance;

while (true)
{
    apiHelper.UpdateUrl();
    Thread.Sleep(2000);
}
