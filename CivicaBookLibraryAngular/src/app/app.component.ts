import { ChangeDetectorRef, Component, OnInit } from '@angular/core';
import { AuthService } from './services/auth.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'CivicaBookLibraryAngular';
  
  isAuthenticated: boolean = true;

  username: string | null | undefined;

  constructor(private authService: AuthService, private cdr: ChangeDetectorRef) {}
  
  ngOnInit(): void {
    this.authService.isAuthenticated().subscribe((authState: boolean)=> {
      this.isAuthenticated=authState;
      this.cdr.detectChanges() 
    });
    this.authService.getUsername().subscribe((username: string | null | undefined)=> {
      this.username = username;
      this.cdr.detectChanges()
    });
  }
  
  signOut(){
    this.authService.signOut();
  }
  
}
