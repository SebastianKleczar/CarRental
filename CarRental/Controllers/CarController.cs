﻿using CarRental.DataBase.Models;
using CarRental.DTO;
using CarRental.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;

namespace CarRental.Controllers
{
    public class CarController : Controller
    {
        private readonly ICarRepository _carRepository;
        private readonly IReviewRepository _reviewRepository;
        private readonly UserManager<Client> userManager;
        public CarController(ICarRepository carRepository, IReviewRepository reviewRepository, UserManager<Client> userManager)
        {
            _carRepository = carRepository;
            _reviewRepository = reviewRepository;
            this.userManager = userManager;
        }
        public IActionResult Index()
        {
            return View();
        }
        public ActionResult GetImg(int carId)
        {
            var byteArray   = _carRepository.GetCarById(carId).Image;
           
            var fileName = $"";

            return File(byteArray, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult Delete(int CarId)
        {
            _carRepository.DeleteCar(CarId);
            return RedirectToAction("GetAvailableCars");
        }


        [HttpGet]
        public IActionResult GetAvailableCars()
        {
            var user = userManager.GetUserAsync(User).Result;
            List<Car> carsFromDb = new List<Car>();
            if (user != null)
            {
                if (userManager.IsInRoleAsync(user, "Admin").Result)
                {
                    carsFromDb = _carRepository.GetAllCars();
                }
                else
                {
                    carsFromDb = _carRepository.GetAllCars().Where(c => c.Availability == true).ToList();
                }
            } else
            {
                carsFromDb = _carRepository.GetAllCars().Where(c => c.Availability == true).ToList();
            }
            
            List<AvailableCarViewModel> carsForView = new List<AvailableCarViewModel>();

            carsFromDb.ForEach(c =>
            carsForView.Add(
                new AvailableCarViewModel()
                {
                    Id = c.Id,
                    Model = c.Model,
                    Brand = c.Brand,
                    Year = c.Year,
                    Colour = c.Colour,
                    Availability = c.Availability,
                    Description = c.Description,
                    Image = c.Image,

                }

                )
            ) ;

            return View(carsForView);
        }


        public IActionResult Details(int CarId)
        {
            Car car =   _carRepository.GetCarById(CarId);
            ViewData["Reviews"] = _reviewRepository.GetReviewsForCar(CarId);
            return View(car);
        }


      
       public Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult AddCar()
        {
            return View(new AddCarDTO());
        }




        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult AddCar(AddCarDTO carDTO)
        {
            //var directory = "../CarRental/Views/Img/";
            var directory = "../CarRental/wwwroot/Img/";
            var car = new Car()
            {
                Brand = carDTO.Brand,
                Description = carDTO.Description,
                Colour = carDTO.Colour,
                Image = SaveFile(carDTO.Image, directory).Result, /*  dodac zapisywanie na dysk  */ /*ToByteArray(carDTO.Image.OpenReadStream()),*/
                Availability = carDTO.Availability,
                Model = carDTO.Model,
                Year = carDTO.Year,

            };

            
            
            _carRepository.AddCar(car);
            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateCar(int CarId)
        {
            Car car = _carRepository.GetCarById(CarId);
            ViewData["Car"] = car;
            var carDto = new AddCarDTO()
            {   Id = car.Id,
                Brand = car.Brand,
                Description = car.Description,
                Colour = car.Colour,
                Model = car.Model,
                Year = car.Year,
                Availability = car.Availability,
            };
            return View(carDto);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateCar(AddCarDTO carDTO)
        {
            //var directory = "../CarRental/Views/Img/";
            var directory = "../CarRental/wwwroot/Img/";

            Car oldCar = _carRepository.GetCarById(carDTO.Id);

            string ImageTemp = oldCar.Image;
            if (carDTO.Image != null) {
                ImageTemp = SaveFile(carDTO.Image, directory).Result; /*  dodac zapisywanie na dysk  */ /*ToByteArray(carDTO.Image.OpenReadStream()),*/
            }
            

            var carToUpdate = new Car()
            {   Id =carDTO.Id,
                Brand = carDTO.Brand,
                Description = carDTO.Description,
                Colour = carDTO.Colour,
                Image = ImageTemp,    
                Availability = carDTO.Availability,
                Model = carDTO.Model,
                Year = carDTO.Year,

            };



            _carRepository.Update(carToUpdate);
            return RedirectToAction("Details", new {CarId = carDTO.Id});
        }








        public async Task<string> SaveFile(IFormFile file, string targetFolder)
        {
            if (file == null || file.Length == 0)
            {
                return "Plik nie został przesłany.";
            }

            // Sprawdź, czy folder docelowy istnieje, jeśli nie, utwórz go
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            // Generuj unikalną nazwę pliku, aby uniknąć konfliktów
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);

            // Ścieżka do zapisu pliku
            var filePath = Path.Combine(targetFolder, fileName);
            Console.WriteLine(filePath);
            Console.WriteLine(Directory.GetCurrentDirectory());
            Console.WriteLine();
            // Zapisz plik
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return fileName;
        }
        public byte[] ToByteArray(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}
