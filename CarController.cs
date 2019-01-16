using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApplicationCore.Interfaces;
using DomainModels.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ViewModels.Base;

namespace SampleAPI.Controllers.Base
{
    public class CarController : BaseController
    {
        #region constructor + variables
        private const string _cacheKey = "car-cache-list";
        private const string _cacheComboKey = "car-cache-combo";
        private readonly IMemoryCache _cache;
        private IAsyncRepository<Car> _repository;
        public CarController(IAsyncRepository<Car> repository, IMemoryCache cache)
        {
            _repository = repository;
            _cache = cache;
        }

        #endregion

        #region actions

        /// <summary>
        /// لیست ماشین ها
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            //read from cache
            if (_cache.TryGetValue(_cacheKey, out IEnumerable<Car> cars))
            {
                return Ok(cars);
            }

            //read from db & write in cache
            var result = await _repository.ListAllAsync();
            _cache.Set(_cacheKey, result);

            return Ok(result);
        }

        /// <summary>
        /// لیست ماشین ها به صورت کومبو
        /// </summary>
        /// <returns></returns>
        [HttpGet("Combo")]
        [AcceptVerbs("Combo")]
        public async Task<IActionResult> GetAllForCombo()
        {
            //read from cache
            if (_cache.TryGetValue(_cacheComboKey, out IEnumerable<KeyValueViewModel> items))
            {
                return Ok(items);
            }

            //read from db & write in cache
            var result = await _repository.ListAllAsync();
            var provinces = result.Select(q => new KeyValueViewModel
            {
                Id = q.ID,
                Title = q.CarName,
            });
            _cache.Set(_cacheComboKey, provinces);

            return Ok(result);
        }

        /// <summary>
        /// اطلاعات ماشین مورد نظر
        /// </summary>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _repository.GetByIdAsync(id);
            return Ok(result);

        }

        /// <summary>
        /// ثبت اطلاعات ماشین جدید
        /// </summary>
        /// <param name="model">مشخصات ماشین</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Post(Car model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repository.AddAsync(model);
            return Ok(model);
        }

        /// <summary>
        /// ویراش اطلاعات ماشین
        /// </summary>
        /// <param name="model">مشخصات ماشین</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Car model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _repository.UpdateAsync(model);
            return Ok(model);
        }

        /// <summary>
        /// حذف ماشین
        /// </summary>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _repository.DeleteAsync(id);
            return Ok();
        }
        #endregion

    }
}
