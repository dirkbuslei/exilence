import 'rxjs/add/operator/map';

import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

import { Player } from '../interfaces/player.interface';
import { Character } from '../interfaces/character.interface';

@Injectable()
export class AccountService {
  public player: BehaviorSubject<Player> = new BehaviorSubject<Player>(undefined);
  public characterList: BehaviorSubject<Character[]> = new BehaviorSubject<Character[]>(undefined);
  constructor() {
  }
  clearCharacterList() {
    this.characterList.next(undefined);
  }
}