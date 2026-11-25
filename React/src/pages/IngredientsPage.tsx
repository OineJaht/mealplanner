import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import { ApiService } from '@/services/api';
import type { Ingredient } from '@/types';

const IngredientsPage: React.FC = () => {
  const [ingredients, setIngredients] = useState<Ingredient[]>([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchIngredients = async () => {
    try {
      setIsLoading(true);
      const response = await ApiService.getIngredients();
      if (response.success && response.data) {
        setIngredients(response.data);
      } else {
        setError(response.message || 'Failed to fetch ingredients');
      }
    } catch (err: any) {
      setError('Failed to fetch ingredients');
    } finally {
      setIsLoading(false);
    }
  };

  const handleSearch = async (term: string) => {
    if (!term.trim()) {
      fetchIngredients();
      return;
    }

    try {
      const response = await ApiService.searchIngredients(term);
      if (response.success && response.data) {
        setIngredients(response.data);
      }
    } catch (err) {
      setError('Failed to search ingredients');
    }
  };

  const handleDelete = async (id: number) => {
    if (!window.confirm('Are you sure you want to delete this ingredient?')) {
      return;
    }

    try {
      const response = await ApiService.deleteIngredient(id);
      if (response.success) {
        setIngredients(ingredients.filter(ingredient => ingredient.id !== id));
      } else {
        setError(response.message || 'Failed to delete ingredient');
      }
    } catch (err) {
      setError('Failed to delete ingredient');
    }
  };

  useEffect(() => {
    fetchIngredients();
  }, []);

  useEffect(() => {
    const timeoutId = setTimeout(() => {
      handleSearch(searchTerm);
    }, 300);

    return () => clearTimeout(timeoutId);
  }, [searchTerm]);

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Header */}
      <div className="flex justify-between items-center">
        <h1 className="text-3xl font-bold text-gray-900">Ingredients</h1>
        <Link
          to="/ingredients/new"
          className="bg-primary-600 hover:bg-primary-700 text-white px-4 py-2 rounded-md text-sm font-medium transition duration-150 ease-in-out"
        >
          Add New Ingredient
        </Link>
      </div>

      {/* Search */}
      <div className="max-w-md">
        <input
          type="text"
          placeholder="Search ingredients..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          className="block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500 sm:text-sm"
        />
      </div>

      {/* Error message */}
      {error && (
        <div className="bg-red-50 border border-red-200 text-red-600 px-4 py-3 rounded-md">
          {error}
        </div>
      )}

      {/* Ingredients grid */}
      {ingredients.length === 0 ? (
        <div className="text-center py-12">
          <p className="text-gray-500 text-lg">No ingredients found.</p>
          <Link
            to="/ingredients/new"
            className="mt-4 inline-block bg-primary-600 hover:bg-primary-700 text-white px-4 py-2 rounded-md text-sm font-medium transition duration-150 ease-in-out"
          >
            Add your first ingredient
          </Link>
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-6">
          {ingredients.map((ingredient) => (
            <div
              key={ingredient.id}
              className="bg-white overflow-hidden shadow rounded-lg border border-gray-200"
            >
              <div className="p-6">
                <div className="flex items-center justify-between">
                  <h3 className="text-lg font-medium text-gray-900 truncate">
                    {ingredient.name}
                  </h3>
                </div>
                <p className="mt-1 text-sm text-gray-500">
                  Category: {ingredient.category || 'Uncategorized'}
                </p>
              </div>
              <div className="bg-gray-50 px-6 py-3 flex justify-between items-center">
                <Link
                  to={`/ingredients/${ingredient.id}/edit`}
                  className="text-primary-600 hover:text-primary-900 text-sm font-medium"
                >
                  Edit
                </Link>
                <button
                  onClick={() => handleDelete(ingredient.id)}
                  className="text-red-600 hover:text-red-900 text-sm font-medium"
                >
                  Delete
                </button>
              </div>
            </div>
          ))}
        </div>
      )}
    </div>
  );
};

export default IngredientsPage;