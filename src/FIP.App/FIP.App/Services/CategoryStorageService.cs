﻿using FIP.App.Constants;
using FIP.Core.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Storage;
using FIP.Core.Models;

namespace FIP.App.Services
{
    class CategoryStorageService : BaseJSONStorageService, ICategoryStorageService
    {
        private IEnumerable<Category> _categories;

        public event EventHandler OnCategoriesUpdated;

        public CategoryStorageService()
        {
            Initialize(Path.Combine(ApplicationData.Current.LocalFolder.Path,
                AppConstants.StorageSettings.StorageFolderName, AppConstants.StorageSettings.CategoriesStorageFileName));

            _categories = GetAllValues<Category>();
        }

        public IEnumerable<Category> Categories
        {
            get => _categories;
            set
            {
                _categories = value;
                SetAllValues(value);
                OnCategoriesUpdated?.Invoke(this, EventArgs.Empty);
            }
        }

        public Category GetCategoryById(Guid id)
        {
            return Categories.SingleOrDefault(x => x.Id == id);
        }

        public Category AddCategory(Category category)
        {
            var categories = Categories.ToList();
            categories.Add(category);
            Categories = categories;

            return category;
        }

        public void DeleteCategoryById(Guid id)
        {
            var categories = Categories.ToList();
            categories.RemoveAll(category => category.Id == id);
            Categories = categories;
        }
    }
}
