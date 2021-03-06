import { Component, OnInit } from '@angular/core';

import { ElectronService } from '../shared/providers/electron.service';
import { LadderService } from '../shared/providers/ladder.service';
import { ActivatedRoute } from '@angular/router';
import { AnalyticsService } from '../shared/providers/analytics.service';

@Component({
  selector: 'app-disconnected',
  templateUrl: './disconnected.component.html',
  styleUrls: ['./disconnected.component.scss']
})
export class DisconnectedComponent implements OnInit {
  private sub: any;
  private external: boolean;
  constructor(
    private electronService: ElectronService,
    private ladderService: LadderService,
    private route: ActivatedRoute,
    private analyticsService: AnalyticsService
  ) { }

  ngOnInit() {
    this.ladderService.stopPollingLadder();

    this.sub = this.route.params.subscribe(params => {
      this.external = JSON.parse(params['external']);
    });

    this.electronService.ipcRenderer.send('disconnect');

    if (this.analyticsService.isTracking) {
      this.analyticsService.sendScreenview('/disconnected');
    }
  }

  openLink(link: string) {
    this.electronService.shell.openExternal(link);
  }

  relaunch() {
    this.electronService.ipcRenderer.send('relaunch');
  }
}
