#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

extern "C" {
    void OpenAppStore_IOS(const char* appID)
    {
        NSString *appIDString = [NSString stringWithUTF8String:appID];
        NSString *urlString = [NSString stringWithFormat:@"https://apps.apple.com/app/id%@", appIDString];
        NSURL *url = [NSURL URLWithString:urlString];

        if (@available(iOS 10.0, *)) {
            [[UIApplication sharedApplication] openURL:url options:@{} completionHandler:nil];
        } else {
            [[UIApplication sharedApplication] openURL:url];
        }
    }
}
