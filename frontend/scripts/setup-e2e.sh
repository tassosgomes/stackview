#!/bin/bash

# E2E Test Setup Script
# This script sets up the environment for running E2E tests locally

set -e

echo "🚀 Setting up E2E test environment..."

# Check if we're in the right directory
if [ ! -f "package.json" ]; then
    echo "❌ Error: Run this script from the frontend directory"
    exit 1
fi

# Install dependencies if not installed
if [ ! -d "node_modules" ]; then
    echo "📦 Installing npm dependencies..."
    npm install
fi

# Install Playwright browsers
echo "🎭 Installing Playwright browsers..."
npx playwright install

# Check if backend is running
echo "🔍 Checking if backend is running..."
if curl -s http://localhost:5096/health > /dev/null; then
    echo "✅ Backend is running"
else
    echo "⚠️  Backend not detected on localhost:5096"
    echo "   Please start the backend before running E2E tests:"
    echo "   cd ../backend && dotnet run --project src/StackShare.API"
fi

# Check if frontend dev server should be started
if curl -s http://localhost:5173 > /dev/null; then
    echo "✅ Frontend dev server is running"
else
    echo "ℹ️  Frontend dev server not running on localhost:5173"
    echo "   The Playwright config will start it automatically"
fi

echo ""
echo "🎯 Ready to run E2E tests!"
echo ""
echo "Available commands:"
echo "  npm run test:e2e        # Run all E2E tests headless"
echo "  npm run test:e2e:headed # Run E2E tests with browser UI"
echo "  npm run test:e2e:ui     # Run with Playwright UI"
echo "  npm run test:e2e:debug  # Debug mode"
echo ""