using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using WebLabsV05.Controllers;
using WebLabsV05.DAL.Entities;
using WebLabsV05.Models;
using Xunit;
using Microsoft.EntityFrameworkCore;
using WebLabsV05.DAL.Data;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using WebLabsV05.Extensions;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace WebLabV5.Tests
{
    public class ProductControllerTests
    {
        //[Theory]
        ////[InlineData(1, 3, 1)] // 1-я страница, кол. объектов 3, id первого объекта 1
        ////[InlineData(2, 2, 4)] // 2-я страница, кол. объектов 2, id первого объекта 4
        //[MemberData(nameof(Data))]

        //public void ControllerGetsProperPage(int page, int qty, int id)
        //{
        //    // Arrange            
        //    var controller = new ProductController();

        //    //controller._dishes = new List<Dish>
        //    //{
        //    //    new Dish{ DishId=1},
        //    //    new Dish{ DishId=2},
        //    //    new Dish{ DishId=3},
        //    //    new Dish{ DishId=4},
        //    //    new Dish{ DishId=5}
        //    //};

        //    controller._dishes = GetDishesList();

        //    // Act
        //    var result = controller.Index(pageNo:page, group:null) as ViewResult;
        //    var model = result?.Model as List<Dish>;

        //    // Assert
        //    Assert.NotNull(model);
        //    Assert.Equal(qty, model.Count);
        //    Assert.Equal(id, model[0].DishId);
        //}

        [Theory]
        //[InlineData(1, 3, 1)] // 1-я страница, кол. объектов 3, id первого объекта 1
        //[InlineData(2, 2, 4)] // 2-я страница, кол. объектов 2, id первого объекта 4
        [MemberData(nameof(Data))]

        public void ControllerGetsProperPage(int page, int qty, int id)
        {
            // Arrange 
            // объекта класса ControllerContext
            var controllerContex = new ControllerContext();  
            // словарь для формирования значений Query
            var queryDictionary = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>();
            queryDictionary.Add("group", "0");
            // макет для TempData
            var tempData = new Mock<ITempDataDictionary>();
            // объект для HttpContext
            var httpContext = new DefaultHttpContext();
            // поместить HttpContext в ControllerContext
            controllerContex.HttpContext = httpContext;
            // сформировать строку запроса
            controllerContex.HttpContext.Request.Query = new QueryCollection(queryDictionary);

            // настройка для сонтекста базы данных
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDb")
                .Options;
            // создать контекст 
            using (var context = new ApplicationDbContext(options))
            {
                // заполнить контекст данными
                context.Dishes.AddRange(
                    new List<Dish>
                        {
                        new Dish{ DishId=1},
                        new Dish{ DishId=2},
                        new Dish{ DishId=3},
                        new Dish{ DishId=4},
                        new Dish{ DishId=5}
                        });
                context.DishGroups.Add(new DishGroup { GroupName = "fake group" });
                context.SaveChanges();           
                // создать объект контроллера
                var controller = new ProductController(context) { ControllerContext=controllerContex };  
                // сформировать объект TempData
                controller.TempData = tempData.Object;

                // Act
                var result = controller.Index(pageNo: page, group: null) as ViewResult;
                var model = result?.Model as List<Dish>;

                // Assert
                Assert.NotNull(model);
                Assert.Equal(qty, model.Count);
                Assert.Equal(id, model[0].DishId);
            }
            using (var context = new ApplicationDbContext(options))
            {
                context.Database.EnsureDeleted();
            }
        }

        //[Fact]
        //public void ControllerSelectsGroup()
        //{
        //    // arrange
        //    var controller = new ProductController();
        //    controller._dishes = GetDishesList();
        //    // act
        //    var result = controller.Index(2) as ViewResult;
        //    var model = result.Model as List<Dish>;
        //    // assert
        //    Assert.Equal(2, model.Count);
        //    Assert.Equal(GetDishesList()[2], 
        //                    model[0], 
        //                    Comparer<Dish>.GetComparer((d1,d2)=> {
        //                        return d1.DishId == d2.DishId;
        //                    }));

        //}


        [Fact]
        
        public void ListViewModelCountsPages()
        {
            // Act
            var model = ListViewModel<Dish>.GetModel(GetDishesList(), 1, 3);

            // Assert
            Assert.Equal(2, model.TotalPages);            
        }
        [Theory]
        [MemberData(memberName: nameof(Data))]
        public void ListViewModelSelectsCorrectQty(int page, int qty, int id)
        {
            // Act
            var model = ListViewModel<Dish>.GetModel(GetDishesList(), page, 3);

            // Assert
            Assert.Equal(qty, model.Count);
        }
        [Theory]
        [MemberData(memberName: nameof(Data))]
        public void ListViewModelHasCorrectData(int page, int qty, int id)
        {
            // Act
            var model = ListViewModel<Dish>.GetModel(GetDishesList(), page, 3);

            // Assert
            Assert.Equal(id, model[0].DishId);
        }
        /// <summary>
        /// Исходные данные для теста
        /// номер страницы, кол.объектов на выбранной странице и
        /// id первого объекта на странице
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> Data()
        {
            yield return new object[] { 1, 3, 1 };
            yield return new object[] { 2, 2, 4 };
        }
        /// <summary>
        /// Получение тестового списка объектов
        /// </summary>
        /// <returns></returns>
        private List<Dish> GetDishesList()
        {
            return new List<Dish>
            {
                new Dish{ DishId=1, DishGroupId=1},
                new Dish{ DishId=2, DishGroupId=1},
                new Dish{ DishId=3, DishGroupId=2},
                new Dish{ DishId=4, DishGroupId=2},
                new Dish{ DishId=5, DishGroupId=3}
            };
        }
    }
}
