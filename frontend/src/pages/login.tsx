import { Card, CardContent } from "@/components/ui/card"
import { Button } from "@/components/ui/button"
import { Input } from "@/components/ui/input"

export function LoginPage() {
  return (
    <div className="flex min-h-[calc(100vh-200px)] items-center justify-center">
      <div className="w-full max-w-md space-y-6">
        <div className="text-center">
          <h1 className="text-2xl font-bold">Sign in to your account</h1>
          <p className="text-muted-foreground mt-2">
            Enter your credentials to access StackShare
          </p>
        </div>
        
        <Card>
          <CardContent className="p-6">
            <form className="space-y-4">
              <div>
                <label htmlFor="email" className="text-sm font-medium">
                  Email
                </label>
                <Input
                  id="email"
                  type="email"
                  placeholder="Enter your email"
                  className="mt-1"
                />
              </div>
              
              <div>
                <label htmlFor="password" className="text-sm font-medium">
                  Password
                </label>
                <Input
                  id="password"
                  type="password"
                  placeholder="Enter your password"
                  className="mt-1"
                />
              </div>
              
              <Button type="submit" className="w-full">
                Sign In
              </Button>
            </form>
            
            <div className="text-center mt-4">
              <a href="/register" className="text-sm text-primary hover:underline">
                Don't have an account? Sign up
              </a>
            </div>
          </CardContent>
        </Card>
      </div>
    </div>
  )
}