using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.NodeServices;
using Microsoft.AspNetCore.NodeServices.HostingModels;
using NodeServicesPlayground.Models;

namespace NodeServicesPlayground.Controllers
{
    public class HomeController : Controller
    {
        private readonly INodeServices _nodeServices;

        public HomeController(INodeServices nodeServices)
        {
            _nodeServices = nodeServices;
        }

        private static string LoremIpsum =>
            @"Lorem ipsum dolor sit amet, consectetur adipiscing elit. In sed blandit mi. Nulla non orci eros. In eu augue eget dolor bibendum mollis. Orci varius natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Aliquam imperdiet nec lorem ac semper. Ut magna libero, lacinia nec libero eget, sollicitudin mattis tortor. Mauris at velit mi. Pellentesque congue sodales sapien, ut pulvinar justo tincidunt quis. Proin posuere risus sed lorem gravida, eu viverra quam vulputate. Sed id mi condimentum, pharetra ex sed, consequat dui.

Sed ac urna faucibus, ultrices turpis eget, posuere purus. Praesent maximus maximus enim sit amet vulputate. Nunc id ligula aliquam, lobortis mi eu, gravida erat. Suspendisse sed ultrices lorem, ac condimentum urna. Suspendisse gravida accumsan erat. Cras ultrices purus et sapien interdum gravida. Cras in ante erat. Nam varius congue egestas. In laoreet leo et vulputate porta. Suspendisse potenti. Aliquam erat volutpat. Vestibulum id cursus massa. Mauris lobortis odio sagittis tortor mollis, ullamcorper gravida tellus feugiat. Suspendisse quis blandit justo. Proin non metus ex.

Pellentesque non tempus ligula. Sed non imperdiet quam. Fusce vel risus ac quam aliquam luctus. In bibendum pulvinar placerat. Vivamus laoreet massa sit amet velit porta lobortis. Ut in nulla ut sem consequat pretium. Curabitur consectetur quis mi eget faucibus. Praesent sit amet quam sit amet augue congue maximus. Quisque viverra a urna ut lacinia. Donec quis quam congue, vestibulum diam ultricies, gravida tortor. Proin eleifend venenatis massa et pellentesque. Aliquam erat volutpat. Sed eget pretium velit, in pellentesque purus.

Pellentesque luctus tortor et mauris pharetra faucibus. Sed sollicitudin, purus ut pulvinar suscipit, massa ante malesuada purus, sed ultrices tellus enim ut nisl. Quisque tellus elit, finibus vel cursus ac, facilisis vel turpis. Cras hendrerit, ligula venenatis semper placerat, lorem diam maximus ex, et pellentesque enim diam eu metus. Sed eget ipsum sem. Vivamus congue, leo ac mattis volutpat, lacus justo pellentesque orci, ut fermentum mauris turpis at lacus. Duis convallis nisi orci, non cursus purus bibendum eget. Integer vitae ultricies arcu. Nam faucibus nibh vitae urna dictum, eget rutrum felis finibus. Donec dictum scelerisque nulla, nec feugiat ante venenatis nec. Fusce pretium tortor est, vel gravida eros hendrerit sit amet.

Maecenas malesuada ex mauris, vitae consequat augue tincidunt in. Mauris a arcu a magna blandit commodo at et lectus. Sed vitae ante convallis, maximus nisi a, elementum purus. Mauris ut tempus ligula. Aliquam ac nibh pharetra, ultricies tellus at, aliquet velit. Cras sit amet blandit libero, nec consequat justo. Fusce vestibulum ultricies porta. Ut lacinia sem sit amet ante pellentesque, at aliquam erat interdum. Donec varius, magna quis luctus semper, sapien lorem imperdiet libero, vel condimentum metus lectus nec libero. Phasellus erat urna, viverra sit amet nunc a, tempor bibendum risus. Etiam convallis ante sed rutrum rutrum. Nunc tortor justo, bibendum a maximus at, dapibus quis ante. Praesent eget sem dolor. Ut sed odio at odio eleifend pretium. Pellentesque at lorem eu sem gravida molestie.";

        public async Task<IActionResult> Index()
        {
            return View();
        }

        public async Task<IActionResult> SingleFunction()
        {
            var result = await _nodeServices.InvokeAsync<string>("./ServerSideJs/function");

            return View(new SingleFunctionViewModel {Data = result});
        }

        public async Task<IActionResult> MultipleFunctions()
        {
            var result1 = await _nodeServices.InvokeExportAsync<string>("./ServerSideJs/multipleFunctions", "returnFive");

            var result2 = await _nodeServices.InvokeExportAsync<string>("./ServerSideJs/multipleFunctions", "returnTen");

            return View(new MultipleFunctionsViewModel {DataFromFunction1 = result1, DataFromFunction2 = result2});
        }

        public async Task<IActionResult> PassingParameters(int param1, int param2)
        {
            var result = await _nodeServices.InvokeAsync<int>("./ServerSideJs/parameters", param1, param2);

            return View(new PassingParametersViewModel {Result = result});
        }

        public async Task<IActionResult> Error()
        {
            var message = string.Empty;

            try
            {
                await _nodeServices.InvokeAsync<int>("./ServerSideJs/error");
            }
            catch (NodeInvocationException e)
            {
                message = e.ToString();
            }

            return View(new ErrorViewModel {Message = message});
        }

        public async Task<IActionResult> MoreRealLife()
        {
            var unsortedArray = new[] {"Przemek", "Emil", "Joanna", "Mateusz", "Belzebub"};
            var sortedArray = await _nodeServices.InvokeAsync<string[]>("./ServerSideJs/sort", new { Array = unsortedArray });

            return View(new MoreRealLifeViewModel { UnsortedNames = unsortedArray, SortedNames = sortedArray});
        }

        public async Task<IActionResult> Pdf()
        {
            try
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                Console.WriteLine($"Before UTC: {DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds}");
                var bytes = await _nodeServices.InvokeAsync<byte[]>("./ServerSideJs/pdf",
                    new
                    {
                        Content = LoremIpsum,
                        PageSize = "A4",
                        Orientation = "Portrait"
                    });
                Console.WriteLine($"After UTC: {DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds}");
                Console.WriteLine($"Creating PDF took: {sw.ElapsedMilliseconds} ms.");

                return File(bytes, "application/pdf", "lorem_ipsum.pdf");
            }
            catch (NodeInvocationException e)
            {
                return View("Error", new ErrorViewModel {Message = e.ToString()});
            }
        }

        public async Task<IActionResult> AudiencePlayground()
        {
            var result = await _nodeServices.InvokeAsync<string>("./ServerSideJs/playground");

            return View(model: result);
        }
    }
}