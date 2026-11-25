import React, { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate, useParams, Link } from 'react-router-dom';
import { ApiService } from '@/services/api';
import type { IngredientDetail, IngredientUpdateDto } from '@/types';

const EditIngredientPage: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const navigate = useNavigate();
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [ingredient, setIngredient] = useState<IngredientDetail | null>(null);

  const {
    register,
    handleSubmit,
    setValue,
    formState: { errors },
  } = useForm<IngredientUpdateDto>();

  const fetchIngredient = async () => {
    if (!id) return;

    try {
      setIsLoading(true);
      const response = await ApiService.getIngredient(parseInt(id));
      
      if (response.success && response.data) {
        setIngredient(response.data);
        // Populate form with existing data
        setValue('id', response.data.id);
        setValue('name', response.data.name);
        setValue('category', response.data.category);
      } else {
        setError(response.message || 'Ingredient not found');
      }
    } catch (err: any) {
      setError('Failed to fetch ingredient');
    } finally {
      setIsLoading(false);
    }
  };

  const onSubmit = async (data: IngredientUpdateDto) => {
    if (!id) return;

    try {
      setIsSubmitting(true);
      setError(null);

      const response = await ApiService.updateIngredient(parseInt(id), data);
      
      if (response.success) {
        navigate('/ingredients');
      } else {
        setError(response.message || 'Failed to update ingredient');
      }
    } catch (err: any) {
      setError(err.response?.data?.message || 'Failed to update ingredient');
    } finally {
      setIsSubmitting(false);
    }
  };

  useEffect(() => {
    fetchIngredient();
  }, [id]);

  if (isLoading) {
    return (
      <div className="flex justify-center items-center h-64">
        <div className="animate-spin rounded-full h-32 w-32 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  if (!ingredient) {
    return (
      <div className="max-w-2xl mx-auto">
        <div className="bg-red-50 border border-red-200 text-red-600 px-4 py-3 rounded-md">
          Ingredient not found
        </div>
        <div className="mt-4">
          <Link
            to="/ingredients"
            className="text-primary-600 hover:text-primary-900 text-sm font-medium"
          >
            ← Back to Ingredients
          </Link>
        </div>
      </div>
    );
  }

  return (
    <div className="max-w-2xl mx-auto">
      <div className="mb-6">
        <div className="flex items-center justify-between">
          <h1 className="text-3xl font-bold text-gray-900">Edit Ingredient</h1>
          <Link
            to="/ingredients"
            className="text-gray-600 hover:text-gray-900 text-sm font-medium"
          >
            ← Back to Ingredients
          </Link>
        </div>
      </div>

      <div className="bg-white shadow rounded-lg">
        <form onSubmit={handleSubmit(onSubmit)} className="p-6 space-y-6">
          {error && (
            <div className="bg-red-50 border border-red-200 text-red-600 px-4 py-3 rounded-md">
              {error}
            </div>
          )}

          <input
            type="hidden"
            {...register('id', { valueAsNumber: true })}
          />

          <div>
            <label htmlFor="name" className="block text-sm font-medium text-gray-700">
              Ingredient Name *
            </label>
            <input
              id="name"
              type="text"
              {...register('name', { required: 'Ingredient name is required' })}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500 sm:text-sm"
              placeholder="Enter ingredient name"
            />
            {errors.name && (
              <p className="mt-1 text-sm text-red-600">{errors.name.message}</p>
            )}
          </div>

          <div>
            <label htmlFor="category" className="block text-sm font-medium text-gray-700">
              Category
            </label>
            <input
              id="category"
              type="text"
              {...register('category')}
              className="mt-1 block w-full px-3 py-2 border border-gray-300 rounded-md shadow-sm focus:outline-none focus:ring-primary-500 focus:border-primary-500 sm:text-sm"
              placeholder="Enter category (e.g., Vegetables, Meat, Dairy)"
            />
            {errors.category && (
              <p className="mt-1 text-sm text-red-600">{errors.category.message}</p>
            )}
          </div>

          <div className="flex justify-end space-x-3 pt-6 border-t border-gray-200">
            <Link
              to="/ingredients"
              className="px-4 py-2 border border-gray-300 rounded-md text-sm font-medium text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500"
            >
              Cancel
            </Link>
            <button
              type="submit"
              disabled={isSubmitting}
              className="px-4 py-2 border border-transparent rounded-md shadow-sm text-sm font-medium text-white bg-primary-600 hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary-500 disabled:opacity-50 disabled:cursor-not-allowed"
            >
              {isSubmitting ? 'Updating...' : 'Update Ingredient'}
            </button>
          </div>
        </form>
      </div>
    </div>
  );
};

export default EditIngredientPage;