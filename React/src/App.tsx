import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider } from '@/contexts/AuthContext';
import ProtectedRoute from '@/components/ProtectedRoute';
import Layout from '@/components/Layout';
import LoginPage from '@/pages/LoginPage';
import RegisterPage from '@/pages/RegisterPage';
import IngredientsPage from '@/pages/IngredientsPage';
import CreateIngredientPage from '@/pages/CreateIngredientPage';
import EditIngredientPage from '@/pages/EditIngredientPage';

const App: React.FC = () => {
  return (
    <AuthProvider>
      <Router>
        <div className="min-h-screen bg-gray-50">
          <Routes>
            {/* Public routes */}
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            
            {/* Protected routes */}
            <Route
              path="/"
              element={
                <ProtectedRoute>
                  <Layout>
                    <Navigate to="/ingredients" replace />
                  </Layout>
                </ProtectedRoute>
              }
            />
            <Route
              path="/ingredients"
              element={
                <ProtectedRoute>
                  <Layout>
                    <IngredientsPage />
                  </Layout>
                </ProtectedRoute>
              }
            />
            <Route
              path="/ingredients/new"
              element={
                <ProtectedRoute>
                  <Layout>
                    <CreateIngredientPage />
                  </Layout>
                </ProtectedRoute>
              }
            />
            <Route
              path="/ingredients/:id/edit"
              element={
                <ProtectedRoute>
                  <Layout>
                    <EditIngredientPage />
                  </Layout>
                </ProtectedRoute>
              }
            />
          </Routes>
        </div>
      </Router>
    </AuthProvider>
  );
};

export default App;